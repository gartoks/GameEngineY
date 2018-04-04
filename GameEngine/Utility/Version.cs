using System;
using System.Linq;

namespace GameEngine.Utility {
    public sealed class Version : IComparable<Version> {

        public static bool TryParseVersionString(string versionString, out Version version) {
            version = null;

            string[] components = versionString.Split(' ');

            if (components.Length > 2)
                return false;

            string postfix = components.Length == 2 ? components[1] : "";

            string[] versionNumbers_raw = components[0].Split('.');

            if (versionNumbers_raw.Length < 2)
                return false;

            if (!uint.TryParse(versionNumbers_raw[0], out uint mainVersionNumber))
                return false;

            if (!uint.TryParse(versionNumbers_raw[1], out uint subVersionNumber))
                return false;

            uint[] additionalVersionNumbers = new uint[versionNumbers_raw.Length - 2];

            for (int i = 2; i < versionNumbers_raw.Length; i++) {
                if (!uint.TryParse(versionNumbers_raw[1], out uint additionalersionNumber))
                    return false;

                additionalVersionNumbers[i - 2] = additionalersionNumber;
            }

            version = new Version(postfix, mainVersionNumber, subVersionNumber, additionalVersionNumbers);
            return true;
        }

        private readonly uint[] versionNumbers;
        private readonly string postfix;

        private readonly string toString;

        public Version(string postfix, uint mainVersion, uint subVersion, params uint[] additionalVersionNumbers) {
            string toStringTmp = "";

            this.versionNumbers = new uint[additionalVersionNumbers.Length + 2];

            this.versionNumbers[0] = mainVersion;
            toStringTmp += mainVersion;
            toStringTmp += "." + subVersion;

            this.versionNumbers[1] = subVersion;

            for (int i = 0; i < additionalVersionNumbers.Length; i++) {
                this.versionNumbers[i + 2] = additionalVersionNumbers[i];
                toStringTmp += "." + additionalVersionNumbers[i];
            }

            this.postfix = postfix ?? "";
            this.toString = toStringTmp;
        }

        public int CompareTo(Version other) {
            int to = System.Math.Min(other.versionNumbers.Length, versionNumbers.Length);
            for (int i = 0; i < to; i++) {
                if (versionNumbers[i] > other.versionNumbers[i])
                    return 1;
                if (versionNumbers[i] < other.versionNumbers[i])
                    return -1;
            }

            return 0;
        }

        public string ToVersionString() => toString;

        public override string ToString() => toString + " " + postfix;

        private bool Equals(Version other) {
            return versionNumbers.SequenceEqual(other.versionNumbers);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Version version && Equals(version);
        }

        public override int GetHashCode() {
            return versionNumbers.GetHashCode();
        }
    }
}