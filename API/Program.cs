using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using API.Data;
using API.Utils;

var builder = WebApplication.CreateBuilder(args);
var connectionString = Utils.getSettings("settings.config");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMySql<SltbetContext>(connectionString[0],Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(connectionString[0]));
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Description = "API for the Sltbetfk project", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
});

app.MapGet("/", () => "i am working :)").WithTags("Teste");

app.MapGet("/Lutadores", async (SltbetContext db) => await db.Lutadores.ToListAsync()).WithTags("Lutadores");
app.MapGet("/Lutadores/{nome}", async (string nome,SltbetContext db) => await db.Lutadores.FindAsync(nome)).WithTags("Lutadores");
app.MapPost("/Lutadores", async (Lutador lutador, SltbetContext db) => {
                                                                        await db.Lutadores.AddAsync(lutador);
                                                                        await db.SaveChangesAsync();
                                                                        return Results.Ok("lutador insirido");
                                                                    }).WithTags("Lutadores");
app.MapPut("/Lutadores", async (LutadorPost lutador, SltbetContext db) => {
                                                                     await db.Lutadores
                                                                     .Where(Lutador => Lutador.Nome == lutador.Nome)
                                                                     .ExecuteUpdateAsync(s =>
                                                                     s.SetProperty(
                                                                     Lutador => Lutador.Vitorias,
                                                                     Lutador => lutador.Vitorias)
                                                                     .SetProperty(
                                                                     Lutador => Lutador.Derrotas,
                                                                     Lutador => lutador.Derrotas)
                                                                     .SetProperty(
                                                                     Lutador => Lutador.Pontos,
                                                                     Lutador => lutador.Pontos)
                                                                     .SetProperty(
                                                                     Lutador => Lutador.Imagem,
                                                                     Lutador => lutador.Imagem)
                                                                     .SetProperty(
                                                                     Lutador => Lutador.Pasta,
                                                                     Lutador => lutador.Pasta));
                                                                     return Results.Ok("Lutador atualizado");
                                                                  }).WithTags("Lutadores");
app.MapPut("/Lutadores/sumpontuacao", async (LutadorPont lutador, SltbetContext db) => {
                                                                     await db.Lutadores
                                                                     .Where(Lutador => Lutador.Nome == lutador.Nome)
                                                                     .ExecuteUpdateAsync(s =>
                                                                     s.SetProperty(
                                                                     Lutador => Lutador.Vitorias,
                                                                     Lutador => Lutador.Vitorias + lutador.Vitorias)
                                                                     .SetProperty(
                                                                     Lutador => Lutador.Derrotas,
                                                                     Lutador => Lutador.Derrotas + lutador.Derrotas)
                                                                     .SetProperty(
                                                                     Lutador => Lutador.Pontos,
                                                                     Lutador => Lutador.Pontos + lutador.Pontos));
                                                                     return Results.Ok("Lutador atualizado");
                                                                  }).WithTags("Lutadores");
app.MapDelete("/Lutadores", async (string nome, SltbetContext db) => {
                                                                    await db.Lutadores.Where(Lutador => Lutador.Nome == nome).ExecuteDeleteAsync();
                                                                    return Results.Ok("Lutador deletado");
                                                                 }).WithTags("Lutadores");
app.MapPatch("/Lutadores/updatetier", async(SltbetContext db) => {
                                                                     var high = await db.Lutadores.MaxAsync(Lutador => Lutador.Pontos) / 9;
                                                                     await db.Lutadores.ExecuteUpdateAsync(s => 
                                                                        s.SetProperty(Lutador => Lutador.Tier, Lutador => 
                                                                        Lutador.Pontos > high * 8 ? "UBER" :
                                                                        Lutador.Pontos > high * 7 ? "SSS"  :
                                                                        Lutador.Pontos > high * 6 ? "SS"   :
                                                                        Lutador.Pontos > high * 5 ? "S"    :
                                                                        Lutador.Pontos > high * 4 ? "A"    :
                                                                        Lutador.Pontos > high * 3 ? "B"    :
                                                                        Lutador.Pontos > high * 2 ? "C"    :
                                                                        Lutador.Pontos > high ? "D" : "F"));
                                                                     return Results.Ok("tiers atualizados");
                                                                  }).WithTags("Lutadores");

app.MapGet("/Lutas", async (SltbetContext db) => await db.Lutas.ToListAsync()).WithTags("Lutas");
app.MapGet("/Lutas/{id}", async (int id, SltbetContext db) => await db.Lutas.Where(Luta => Luta.LutaId == id).ToListAsync()).WithTags("Lutas");
app.MapGet("/Lutas/data/{data}", async (string data, SltbetContext db) => {
                                                                              await db.Lutas.Where(Luta => Luta.Data == DateTime.Parse(data)).ToListAsync();
                                                                           }).WithTags("Lutas");
app.MapPost("/Lutas", async (Luta luta, SltbetContext db) => {
                                                               await db.Lutas.AddAsync(luta);
                                                               await db.SaveChangesAsync();
                                                               return Results.Ok("Luta inserida");
                                                             }).WithTags("Lutas");
app.MapDelete("/Lutas", async (int id, SltbetContext db) => {
                                                                     await db.Lutas.Where(Luta => Luta.LutaId == id).ExecuteDeleteAsync();
                                                                     return Results.Ok("Luta deletada");
                                                                  }).WithTags("Lutas");;

app.Run();


