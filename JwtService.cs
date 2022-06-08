using System;

public class JwtService : IJwtService
{
    public string CreateToken(ApplicationUser applicationUser)

    {

        var claims = new List<Claim>

            {

                new Claim(JwtRegisteredClaimNames.NameId, applicationUser.UserName)

            };



        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(""));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var descriptor = new SecurityTokenDescriptor

        {

            Subject = new ClaimsIdentity(claims),

            Expires = DateTime.Now.AddHours(1),

            SigningCredentials = credentials,

        };



        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateToken(descriptor);

        return handler.WriteToken(token);

    }
    public JwtService()
    {
    }
}
