using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Data;

public partial class Lutador
{
    public string Nome { get; set; } = null!;

    public int Vitorias { get; set; }

    public int Derrotas { get; set; }

    public int Pontos { get; set; }

    public string Tier { get; set; } = null!;

    public string? Imagem { get; set; }

    public string Pasta { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Luta> LutaLutador1Navigations { get; set; } = new List<Luta>();
    [JsonIgnore]
    public virtual ICollection<Luta> LutaLutador2Navigations { get; set; } = new List<Luta>();
}
