
using Candidate;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var origenespermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(',');

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PoliticaCors", politica =>
    {
        politica.WithOrigins(origenespermitidos)
                .AllowAnyHeader()
                .AllowAnyMethod().AllowCredentials();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PoliticaCors");
//app.MapHub<Charthub>("/charthub");
app.UseAuthorization();

app.MapControllers();

app.Run();
