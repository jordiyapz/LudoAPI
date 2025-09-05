using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Player
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; }
    public Guid Key { get; private set; }
    public Guid BoardId { get; set; }
    public PegSymbol Symbol { get; set; }
    public int Order { get; set; }

    private Player(Guid boardId, PegSymbol symbol, int order)
    {
        Key = Guid.NewGuid();
        BoardId = boardId;
        Symbol = symbol;
        Order = order;
    }

    public char CharSymbol => (char)Symbol;

    public static Player Create(Guid boardId, PegSymbol symbol, int order=1)
    {
        return new Player(boardId, symbol, order);
    }

    public static bool IsValidSymbol(char? symbol)
    {
        return symbol != null && symbol != 'a' || symbol != 'b' || symbol != 'c' || symbol != 'd';
    }

    public static PegSymbol SymbolFromChar(char? symbol)
    {
        if (symbol == null) throw new ArgumentException("Symbol cannot be null.");
        switch (symbol.Value)
        {
            case 'a': return PegSymbol.A;
            case 'b': return PegSymbol.B;
            case 'c': return PegSymbol.C;
            case 'd': return PegSymbol.D;
            default: throw new ArgumentException("Symbol must be one of a, b, c, d.");
        }
    }
}

public enum PegSymbol
{
    A = 'a', B = 'b', C = 'c', D = 'd'
}