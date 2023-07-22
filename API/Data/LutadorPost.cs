using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Data;

public partial class LutadorPost
{
    public string Nome { get; set; } = null!;

    public int Vitorias { get; set; }

    public int Derrotas { get; set; }

    public int Pontos { get; set; }

    public string? Imagem { get; set; }

    public string Pasta { get; set; } = null!;
}
