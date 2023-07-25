using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using API.Data;
using API.Utils;

var builder = WebApplication.CreateBuilder(args);
var connectionString = Utils.getSettings("settings.config");

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorizationBuilder()
  .AddPolicy("admin", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("scope", "APP"));
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMySql<SltbetContext>(connectionString[0],Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(connectionString[0]));

builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "SltbetFK", Description = "API for the Sltbetfk project"});
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
});

app.MapGet("/", () => "up and working").WithTags("Debug");
app.MapGet("/auth", () => "Authorized").WithTags("Debug").RequireAuthorization("admin");

app.MapGet("/Lutadores", async (SltbetContext db) => await db.Lutadores.ToListAsync()).WithTags("Lutadores")
                                                                                      .Produces<IEnumerable<Lutador>>(200);
app.MapGet("/Lutadores/{nome}", async (string nome,SltbetContext db) => {
                                                                           var l = await db.Lutadores.FindAsync(nome);
                                                                           if (l == null) return Results.NoContent();
                                                                           return Results.Ok<Lutador>(l);
                                                                        }).WithTags("Lutadores")
                                                                          .Produces<Lutador>(200).Produces(204);
app.MapPost("/Lutadores", async (Lutador lutador, SltbetContext db) => {

                                                                        if(db.Lutadores.FirstOrDefault(l => l.Nome == lutador.Nome) != null){
                                                                           return Results.BadRequest("Lutador já existe");
                                                                        }

                                                                        await db.Lutadores.AddAsync(lutador);
                                                                        await db.SaveChangesAsync();
                                                                        return Results.CreatedAtRoute("PostLutador", new {nome = lutador.Nome}, lutador);
                                                                        
                                                                    }).WithName("PostLutador").WithTags("Lutadores")
                                                                      .Produces<Lutador>(201).Produces(400).Produces(401)
                                                                      .RequireAuthorization("admin");
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
                                                                  }).WithTags("Lutadores")
                                                                    .Produces(200).Produces(400).Produces(401)
                                                                    .RequireAuthorization("admin");
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
                                                                  }).WithTags("Lutadores")
                                                                    .Produces(200).Produces(400).Produces(401)
                                                                    .RequireAuthorization("admin");
app.MapDelete("/Lutadores", async (string nome, SltbetContext db) => {
                                                                    await db.Lutadores.Where(Lutador => Lutador.Nome == nome).ExecuteDeleteAsync();
                                                                    return Results.Ok("Lutador deletado");
                                                                 }).WithTags("Lutadores")
                                                                   .Produces(200).Produces(400).Produces(401)
                                                                   .RequireAuthorization("admin");
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
                                                                  }).WithTags("Lutadores")
                                                                    .Produces(200).Produces(401)
                                                                    .RequireAuthorization("admin");

app.MapGet("/Lutas", async (SltbetContext db) => await db.Lutas.ToListAsync()).WithTags("Lutas")
                                                                              .Produces<IEnumerable<Luta>>(200);
app.MapGet("/Lutas/{id}", async (int id, SltbetContext db) => {
                                                               var l = await db.Lutas.Where(Luta => Luta.LutaId == id).ToListAsync();
                                                               if(!l.Any()) return Results.NoContent();
                                                               return Results.Ok(l);
                                                               }).WithTags("Lutas")
                                                                 .Produces<Luta>(200).Produces(204);
app.MapGet("/Lutas/data/{data}", async (string data, SltbetContext db) => {
                                                                              await db.Lutas.Where(Luta => Luta.Data == DateTime.Parse(data)).ToListAsync();
                                                                           }).WithTags("Lutas");
app.MapPost("/Lutas", async (Luta luta, SltbetContext db) => {
                                                               if(db.Lutas.FirstOrDefault(l => l.LutaId == luta.LutaId) != null){
                                                                  return Results.BadRequest("Luta já existe");
                                                               }
                                                               await db.Lutas.AddAsync(luta);
                                                               await db.SaveChangesAsync();
                                                               return Results.CreatedAtRoute("PostLuta", new {lutaid = luta.LutaId}, luta);
                                                             }).WithName("PostLuta").WithTags("Lutas")
                                                               .Produces<Luta>(201).Produces(400).Produces(401)
                                                               .RequireAuthorization("admin");
app.MapDelete("/Lutas", async (int id, SltbetContext db) => {
                                                               await db.Lutas.Where(Luta => Luta.LutaId == id).ExecuteDeleteAsync();
                                                               return Results.Ok("Luta deletada");
                                                            }).WithTags("Lutas")
                                                              .Produces(200).Produces(400).Produces(401)
                                                              .RequireAuthorization("admin");

app.Run();


