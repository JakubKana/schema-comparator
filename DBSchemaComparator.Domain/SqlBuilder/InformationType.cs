namespace DBSchemaComparator.Domain.SqlBuilder
{
    public enum InformationType
    {
        Tables,
        Columns,
        IdentityColumns,
        Indexes,
        StoredProcedure,
        Function,
        View,
        Checks,
        PrimaryKeys,
        ForeignKeys,
        DatabaseCollation
    }
}