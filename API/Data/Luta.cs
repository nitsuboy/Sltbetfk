using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Data;

public partial class Luta
{
    public int LutaId { get; set; }

    public string Lutador1 { get; set; } = null!;

    public string Lutador2 { get; set; } = null!;

    public DateTime Data { get; set; }

    public int Vencendor { get; set; }
    [JsonIgnore]
    public virtual Lutador Lutador1Navigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Lutador Lutador2Navigation { get; set; } = null!;
}
