using PFC.API.Extensions;
using PFC.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PFC API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandling();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
