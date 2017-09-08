using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Types
{
    /// <summary>
    /// Immutable Class for primary key containing a Yead+Id
    /// </summary>
    public class YearId : IEquatable<YearId>, IEqualityComparer<YearId>
    {
        public short Year
        {
            get;
            private set;
        }

        public int Id
        {
            get;
            private set;
        }

        public YearId(short year, int id)
        {
            Year = year;
            Id = id;
        }

        public static YearId Create(short year, int id)
        {
            return new YearId(year, id);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is YearId))
                return false;
            YearId datedId = obj as YearId;
            return Equals(datedId);
        }

        public bool Equals(YearId other)
        {
            return ((this.Year == other.Year) && (this.Id == other.Id));
        }

        public bool Equals(YearId x, YearId y)
        {
            return ((x.Year == y.Year) && (x.Id == y.Id));
        }

        public int GetHashCode(YearId obj)
        {
            return (obj.Year + "Year" + obj.Id + "Id").GetHashCode();
        }

        public override string ToString()
        {
            return "Year=" + Year + ", Id=" + Id;
        }
    }
}
