namespace EscolaQApabilities.StudentService.Infrastructure.Configuration;

public static class DatabaseConfiguration
{
    /// <summary>
    /// Collation padrão para campos de texto que precisam de busca case-insensitive
    /// </summary>
    public const string DefaultCollation = "SQL_Latin1_General_CP1_CI_AS";
    
    /// <summary>
    /// Collation para campos que não precisam de busca case-insensitive
    /// </summary>
    public const string BinaryCollation = "SQL_Latin1_General_CP1_CS_AS";
    
    /// <summary>
    /// Collation para campos de email (case-insensitive)
    /// </summary>
    public const string EmailCollation = DefaultCollation;
    
    /// <summary>
    /// Collation para campos de nome (case-insensitive)
    /// </summary>
    public const string NameCollation = DefaultCollation;
    
    /// <summary>
    /// Collation para campos de cidade (case-insensitive)
    /// </summary>
    public const string CityCollation = DefaultCollation;
    
    /// <summary>
    /// Collation para campos de estado (case-insensitive)
    /// </summary>
    public const string StateCollation = DefaultCollation;
    
    /// <summary>
    /// Collation para campos de role (case-insensitive)
    /// </summary>
    public const string RoleCollation = DefaultCollation;
}

