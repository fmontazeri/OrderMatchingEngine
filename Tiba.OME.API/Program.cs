using System.Text.Json.Serialization;
using Tiba.OME.API.Components;
using Tiba.OME.Application.CommandHandlers;
using Tiba.OME.Application.Contracts;
using Tiba.OME.Application.Contracts.Commands;
using Tiba.OME.Application.Contracts.Services;
using Tiba.OME.Application.Services;
using Tiba.OME.Domain.OrderBookAgg;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IOrderBookService, OrderBookService>();
builder.Services.AddTransient<IOrderBookRepository, OrderBookRepository>();
//builder.Services.AddScoped(typeof(ICommandHandler<>), typeof(ICommandHandler<AddOrderCommand>));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();
app.Run();