using System;

namespace WebApplication.AthenaCore.SQLite.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        private readonly ColumnFlags columnFlags;

        public ColumnAttribute() { }
        
        public ColumnAttribute(ColumnFlags columnFlags)
        {
            this.columnFlags = columnFlags;
        }

        public bool HasFlag(ColumnFlags flag)
        {
            return (columnFlags & flag) != ColumnFlags.None;
        }
    }

    [Flags]
    public enum ColumnFlags
    {
        //Values are powers of 2, so that they each get their own 'column' in bits.
        None = 0,
        PrimaryKey = 1,
        Required = 2,
        NotNull = 4
    }
}