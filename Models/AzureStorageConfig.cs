using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Models;

public class AzureStorageConfig
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
    
    [Required]
    public string ContainerName { get; set; } = string.Empty;
} 