using JetBrains.Util.PersistentMap;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using NuGet.Frameworks;
using JetBrains.Annotations;

namespace JetBrains.Application.platforms
{
	[Localizable(false)]
	public sealed class PlatformID : IEquatable<PlatformID>
	{
		private readonly FrameworkIdentifier myIdentifier;
		private readonly Version myVersion;
		private readonly ProfileIdentifier myProfile;
		private string myFullName;

		private PlatformID(string frameworkName)
		{
			if (frameworkName == null)
				throw new ArgumentNullException(nameof(frameworkName));
			if (frameworkName.Length == 0)
				throw new ArgumentException(string.Format("{0} is empty after trim", (object)nameof(frameworkName)));
			string[] strArray1 = frameworkName.Split(',');
			if (strArray1.Length < 2 || strArray1.Length > 3)
				throw new ArgumentException("FramewWorkName is too short");
			string identifier = strArray1[0].Trim();
			if (identifier.Length == 0)
				throw new ArgumentException("FramewWorkName is invalid");
			this.myIdentifier = new FrameworkIdentifier(identifier);
			bool flag = false;
			this.myProfile = new ProfileIdentifier(string.Empty);
			for (int index = 1; index < strArray1.Length; ++index)
			{
				string[] strArray2 = strArray1[index].Split('=');
				if (strArray2.Length != 2)
					throw new ArgumentException("FramewWorkName is invalid");
				string str1 = strArray2[0].Trim();
				string str2 = strArray2[1].Trim();
				if (str1.Equals(nameof(Version), StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					if (str2.Length > 0 && (str2[0] == 'v' || str2[0] == 'V'))
						str2 = str2.Substring(1);
					try
					{
						this.myVersion = new Version(str2);
						if (this.myVersion.Build == 0)
							this.myVersion = new Version(this.myVersion.Major, this.myVersion.Minor);
					}
					catch (Exception ex)
					{
						throw new ArgumentException("FramewWorkName is invalid", ex);
					}
				}
				else
				{
					if (!str1.Equals(nameof(Profile), StringComparison.OrdinalIgnoreCase))
						throw new ArgumentException("FramewWorkName is invalid");
					if (!string.IsNullOrEmpty(str2))
						this.myProfile = new ProfileIdentifier(str2);
				}
			}
			if (!flag)
				throw new ArgumentException("FrameWorkName is missing");
		}

		public PlatformID([NotNull] NuGetFramework nugetFramework)
		  : this(nugetFramework.DotNetFrameworkName)
		{
		}

		public PlatformID([NotNull] FrameworkIdentifier identifier, [NotNull] Version version, [CanBeNull] ProfileIdentifier profile = null)
		{
			if (identifier == (FrameworkIdentifier)null)
				throw new ArgumentNullException(nameof(identifier));
			if (version == (Version)null)
				throw new ArgumentNullException(nameof(version));
			this.myIdentifier = identifier;
			this.myVersion = (Version)version.Clone();
			ProfileIdentifier profileIdentifier = profile;
			if ((object)profileIdentifier == null)
				profileIdentifier = ProfileIdentifier.Default;
			this.myProfile = profileIdentifier;
		}

		public static PlatformID TryCreate(string frameworkName)
		{
			try
			{
				return new PlatformID(frameworkName);
			}
			catch (ArgumentException ex)
			{
				return (PlatformID)null;
			}
		}

		public string FullName
		{
			get
			{
				if (this.myFullName == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append((object)this.Identifier);
					stringBuilder.Append(',');
					stringBuilder.Append("Version").Append('=');
					stringBuilder.Append('v');
					stringBuilder.Append((object)this.Version);
					if (!this.Profile.IsDefault())
					{
						stringBuilder.Append(',');
						stringBuilder.Append("Profile").Append('=');
						stringBuilder.Append((object)this.Profile);
					}
					this.myFullName = stringBuilder.ToString();
				}
				return this.myFullName;
			}
		}

		public ProfileIdentifier Profile
		{
			get
			{
				return this.myProfile;
			}
		}

		public FrameworkIdentifier Identifier
		{
			get
			{
				return this.myIdentifier;
			}
		}

		public Version Version
		{
			get
			{
				return this.myVersion;
			}
		}

		public static PlatformID CreateFromName(
		  [NotNull] string name,
		  [CanBeNull] Version version = null,
		  ProfileIdentifier profile = null)
		{
			return PlatformID.CreateFromName(new FrameworkIdentifier(name), version, profile);
		}

		public static PlatformID CreateFromName(
		  [NotNull] FrameworkIdentifier framework,
		  [CanBeNull] Version version = null,
		  ProfileIdentifier profile = null)
		{
			FrameworkIdentifier identifier = framework;
			Version version1 = version;
			if ((object)version1 == null)
				version1 = new Version();
			ProfileIdentifier profile1 = profile;
			return new PlatformID(identifier, version1, profile1);
		}

		public bool Equals(PlatformID other)
		{
			if ((object)other == null)
				return false;
			if ((object)this == (object)other)
				return true;
			if (object.Equals((object)other.myIdentifier, (object)this.myIdentifier) && object.Equals((object)other.myProfile, (object)this.myProfile))
				return object.Equals((object)other.myVersion, (object)this.myVersion);
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if ((object)this == obj)
				return true;
			if (obj.GetType() != typeof(PlatformID))
				return false;
			return this.Equals((PlatformID)obj);
		}

		public static bool operator ==(PlatformID left, PlatformID right)
		{
			return object.Equals((object)left, (object)right);
		}

		public static bool operator !=(PlatformID left, PlatformID right)
		{
			return !object.Equals((object)left, (object)right);
		}

		public override int GetHashCode()
		{
			return this.Identifier.GetHashCode() ^ this.Version.GetHashCode() ^ this.Profile.GetHashCode();
		}

		public override string ToString()
		{
			return this.FullName;
		}

		public void WritePlatformId(BinaryWriter writer)
		{
			this.myIdentifier.WriteFrameworkIdentifier(writer);
			writer.Write(this.myVersion.ToString());
			this.myProfile.WriteProfileIdentifier(writer);
		}

		public static PlatformID ReadPlatformId(BinaryReader reader)
		{
			return new PlatformID(new FrameworkIdentifier(reader.ReadString()), new Version(reader.ReadString()), new ProfileIdentifier(reader.ReadString()));
		}

		public void WritePlatformId(UnsafeWriter writer)
		{
			this.myIdentifier.WriteFrameworkIdentifier(writer);
			writer.Write(this.myVersion.ToString());
			this.myProfile.WriteProfileIdentifier(writer);
		}

		public static PlatformID ReadPlatformId(UnsafeReader reader)
		{
			return new PlatformID(new FrameworkIdentifier(reader.ReadString()), new Version(reader.ReadString()), new ProfileIdentifier(reader.ReadString()));
		}
	}
}
