using AutoMapper.Extensions.ExpressionMapping;
using MessagingService.BLL;
using MessagingService.BLL.Implementations;
using MessagingService.DAL;
using MessagingService.DAL.ContextInfo;
using MessagingService.DAL.Implementations;
using MessagingService.Entity.Mappings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCon"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(x =>
{
    x.AddExpressionMapping(); //expressionlarý maplemek içindir
    x.AddProfile(typeof(Maps));
});

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMessageRepo, MessageRepo>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddScoped<IBlockListRepo, BlockListRepo>();
builder.Services.AddScoped<IBlockListService, BlockListService>();


builder.Services.AddScoped<IActivityLogListRepo, ActivityLogListRepo>();
builder.Services.AddScoped<IActivityLogListService, ActivityLogListService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
