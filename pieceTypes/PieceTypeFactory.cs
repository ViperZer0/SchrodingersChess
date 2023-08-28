using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

public static class PieceTypeFactory
{
    public static IEnumerable<IPieceType> GetAllPieceTypes()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        
        // Get all IPieceType implementers (each of the piece types)
        IEnumerable<Type> pieceTypes = assembly.GetTypes().Where(t => typeof(IPieceType).IsAssignableFrom(t) && !t.IsInterface).Select(t => t);

        // Instantiate each type and return the collection.
        return pieceTypes.Select(t => Activator.CreateInstance(t) as IPieceType);
    }
}
