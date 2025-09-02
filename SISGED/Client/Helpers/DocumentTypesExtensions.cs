using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Client.Helpers;

public static class DocumentTypesExtensions
{
    public static string GetType(Type document)
    {
        if(document.BaseType is null || document.BaseType.Name != nameof(Document))
            throw new ArgumentException("The provided type is not a valid Document type.");
        
        var documentType = document.GetCustomAttribute<BsonDiscriminatorAttribute>();

        ArgumentNullException.ThrowIfNull(documentType);

        return documentType.Discriminator;
    }
}