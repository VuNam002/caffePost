using CaffePOS.Data; 
using CaffePOS.Services; 
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; 
using Microsoft.AspNetCore.Diagnostics; 
using System.Text.Json; 

// Định nghĩa tên chính sách CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")));
builder.Services.AddScoped<ItemsService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<RolePermissionService>();
builder.Services.AddScoped<OrderItemService>();

// 1.3 Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// 1.4 Cấu hình JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Token failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 1.7 Cấu hình Swagger hỗ trợ JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CaffePOS API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 1.8 Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // Cho phép URL của React (Vite default)
                          policy.WithOrigins("http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod();

                          // Bạn có thể thêm các URL khác nếu cần
                          // policy.WithOrigins("http://localhost:3000", "https://my-domain.com")
                      });
});


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    // Hiển thị trang lỗi chi tiết khi phát triển
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(appError =>
    {
        appError.Run(async context =>
        {
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (contextFeature != null)
            {
                // (Ghi log lỗi ở đây)
                // logger.LogError(contextFeature.Error, "Unhandled exception");
            }

            // Trả về response lỗi JSON thống nhất
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                success = false,
                message = "Đã có lỗi hệ thống xảy ra."
            }));
        });
    });
}

// Chuyển hướng HTTP sang HTTPS
//app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
























//using CaffePOS.Data;
//using CaffePOS.Services;
//using Microsoft.EntityFrameworkCore;
//using System.Text;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;

//var builder = WebApplication.CreateBuilder(args);

//// 1. Đăng ký DbContext
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")));

//// 2. Đăng ký các Service
//builder.Services.AddScoped<ItemsService>();
//builder.Services.AddScoped<CategoryService>();
//builder.Services.AddScoped<UserService>();
//builder.Services.AddScoped<RoleService>();
//builder.Services.AddScoped<OrderService>();
//builder.Services.AddScoped<PaymentService>();
//builder.Services.AddScoped<PermissionService>();
//builder.Services.AddScoped<RolePermissionService>();
//builder.Services.AddScoped<OrderItemService>();

//// 3. Cấu hình JWT Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.Events = new JwtBearerEvents
//        {
//            OnAuthenticationFailed = context =>
//            {
//                Console.WriteLine($"Token failed: {context.Exception.Message}");
//                return Task.CompletedTask;
//            }
//        };
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });


//builder.Services.AddAuthorization(); 

//// 4. Đăng ký Controllers và Swagger
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();

//// 5. Cấu hình Swagger hỗ trợ JWT
//builder.Services.AddSwaggerGen(c =>
//{
//    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
//        Name = "Authorization",
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//    {
//        {
//            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//            {
//                Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                {
//                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//var app = builder.Build();

//// 6. HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();


//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();

