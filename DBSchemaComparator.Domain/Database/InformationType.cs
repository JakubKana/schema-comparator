namespace DBSchemaComparator.Domain.Database
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
        PrimaryKeys
    }
}