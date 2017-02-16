--select * from [sys].[index_columns];
--select * from [INFORMATION_SCHEMA].CHECK_CONSTRAINTS;
select * from [INFORMATION_SCHEMA].CONSTRAINT_COLUMN_USAGE


SELECT so.name AS TableName, si.name AS IndexName, si.type_desc AS IndexType, si.is_primary_key AS IsPrimaryKey, si.is_unique as IsUnique, c.name AS ColumnName
FROM
sys.indexes si
            INNER JOIN sys.objects so ON si.[object_id] = so.[object_id]
			INNER JOIN sys.index_columns ic ON si.object_id = ic.object_id AND ic.index_id = si.index_id
			INNER JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE
			so.type = 'U'    --Only get indexes for User Created Tables
            AND 
            si.name IS NOT NULL;

SELECT OBJECT_NAME(OBJECT_ID) AS NameofConstraint,
SCHEMA_NAME(schema_id) AS SchemaName,
OBJECT_NAME(parent_object_id) AS TableName,
type_desc AS ConstraintType
FROM sys.objects
WHERE type_desc LIKE '%CONSTRAINT'

