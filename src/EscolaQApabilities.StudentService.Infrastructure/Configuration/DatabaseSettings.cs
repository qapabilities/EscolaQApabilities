namespace EscolaQApabilities.StudentService.Infrastructure.Configuration;

public class DatabaseSettings
{
    public const string SectionName = "Database";
    
    /// <summary>
    /// Collation padrão para campos de texto que precisam de busca case-insensitive
    /// </summary>
    public string DefaultCollation { get; set; } = DatabaseConfiguration.DefaultCollation;
    
    /// <summary>
    /// Collation para campos que não precisam de busca case-insensitive
    /// </summary>
    public string BinaryCollation { get; set; } = DatabaseConfiguration.BinaryCollation;
    
    /// <summary>
    /// Collation para campos de email (case-insensitive)
    /// </summary>
    public string EmailCollation { get; set; } = DatabaseConfiguration.EmailCollation;
    
    /// <summary>
    /// Collation para campos de nome (case-insensitive)
    /// </summary>
    public string NameCollation { get; set; } = DatabaseConfiguration.NameCollation;
    
    /// <summary>
    /// Collation para campos de cidade (case-insensitive)
    /// </summary>
    public string CityCollation { get; set; } = DatabaseConfiguration.CityCollation;
    
    /// <summary>
    /// Collation para campos de estado (case-insensitive)
    /// </summary>
    public string StateCollation { get; set; } = DatabaseConfiguration.StateCollation;
    
    /// <summary>
    /// Collation para campos de role (case-insensitive)
    /// </summary>
    public string RoleCollation { get; set; } = DatabaseConfiguration.RoleCollation;
    
    /// <summary>
    /// Valida as configurações do banco de dados
    /// </summary>
    public void Validate()
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(DefaultCollation))
            errors.Add("DefaultCollation is required");
            
        if (string.IsNullOrWhiteSpace(EmailCollation))
            errors.Add("EmailCollation is required");
            
        if (string.IsNullOrWhiteSpace(NameCollation))
            errors.Add("NameCollation is required");
            
        if (string.IsNullOrWhiteSpace(CityCollation))
            errors.Add("CityCollation is required");
            
        if (string.IsNullOrWhiteSpace(StateCollation))
            errors.Add("StateCollation is required");
            
        if (string.IsNullOrWhiteSpace(RoleCollation))
            errors.Add("RoleCollation is required");
            
        if (errors.Any())
        {
            throw new InvalidOperationException($"Database configuration validation failed: {string.Join("; ", errors)}");
        }
    }
}

