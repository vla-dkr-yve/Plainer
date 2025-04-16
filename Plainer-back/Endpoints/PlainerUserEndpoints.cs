using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Plainer.Configuration;
using Plainer.Data;
using Plainer.DTOs.UserDTOs;
using Plainer.Entities;
using Plainer.Helper;

namespace Plainer.Endpoints;

public static class PlainerUserEndpoints
{
    public static RouteGroupBuilder MapPlainerUserEndpoints(this WebApplication app){

        var group = app.MapGroup("users");


        //register
        group.MapPost("/register",async (UserRegisterDTO UserRegisterDTO,PlainerDbContext dbContext) => 
            {
                if(await dbContext.Users.AnyAsync(x=>x.Username == UserRegisterDTO.Username)){
                    return Results.BadRequest("Username is taken");
                }

                User user = new();
                var hashedPassword = new PasswordHasher<User>().HashPassword(user,UserRegisterDTO.Password);

                user.Username = UserRegisterDTO.Username;
                user.PasswordHash = hashedPassword;

                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();

                return Results.Ok();
            }
        );

        //Login
        group.MapPost("/login",async (UserRegisterDTO UserRegisterDTO,PlainerDbContext dbContext, IOptions<JwtSettings> options) => 
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x=>x.Username == UserRegisterDTO.Username);

                if(user is null)
                    return Results.BadRequest("User doesn't exists");

                if(new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, UserRegisterDTO.Password)
                == PasswordVerificationResult.Failed)
                    return Results.BadRequest("Incorrect password");;

                var token = JwtHelper.GenerateToken(user, options.Value);

                return Results.Ok(new { token } );
            }
        );

        return group;
    }
}
