using AutoMapper;
using manto_stock_system_API.DTOs;
using manto_stock_system_API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static manto_stock_system_API.Utils.Constants;
using manto_stock_system_API.Extensions;

namespace manto_stock_system_API.Services
{
    public class UserServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public UserServices(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper)
        {
            this._context = context;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._mapper = mapper;
        }

        public async Task<AuthResponseDTO> CreateUser(ApplicationUserCreationDTO applicationUserCreationDTO)
        {
            var user = _mapper.Map<ApplicationUser>(applicationUserCreationDTO);
            var result = await _userManager.CreateAsync(user, applicationUserCreationDTO.Password);

            if (result.Succeeded)
            {
                return await BuildToken(user);
            }
            else
            {
                throw new BadHttpRequestException(result.Errors.ToString());
            }
        }

        public async Task<ListResponse<ApplicationUserDTO>> GetUsers(
            BaseFilter baseFilter)
        {
            var users = _context.ApplicationUsers
                .AsQueryable();

            return await users.FilterSortPaginate<ApplicationUser, ApplicationUserDTO>(
                baseFilter, _mapper);
        }

        public async Task<ApplicationUserDTO> GetUser(Guid id)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(a => a.Id == id.ToString());

            if (user == null)
            {
                return null;
            }

            return _mapper.Map<ApplicationUserDTO>(user);
        }

        public async Task<AuthResponseDTO> Login(LoginDTO loginDTO)
        {
            var error = BuildErrorResponse("INVALID_CREDENTIALS",
                "Invalid credentials");
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null) return error;

            var result = await _signInManager.CheckPasswordSignInAsync(user,
                loginDTO.Password, false);

            if (result.Succeeded) return await BuildToken(user);

            return error;
        }

        private async Task<AuthResponseDTO> BuildToken(ApplicationUser applicationUser)
        {
            var claims = new List<Claim>()
            {
                new (CustomClaims.ID_CLAIM_TYPE, applicationUser.Id),
                new (CustomClaims.EMAIL_CLAIM_TYPE, applicationUser.Email)
            };

            if (applicationUser.UserType == UserTypeEnum.Admin)
            {
                claims.Add(new Claim(ClaimTypes.Role, Roles.ADMIN));
            }
            if (applicationUser.UserType == UserTypeEnum.User)
            {
                claims.Add(new Claim(ClaimTypes.Role, Roles.USER));
            }

            var claimsDB = await _userManager.GetClaimsAsync(applicationUser);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("MANTO_JWT_KEY")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiracion,
                signingCredentials: creds);

            var respuesta = new AuthResponseDTO
            {
                AuthToken =
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        ExpiresIn = (int)(expiracion - DateTime.UtcNow).TotalMinutes
                    },
                TokenType = "Bearer",
                AuthState = _mapper.Map<ApplicationUserDTO>(applicationUser)
            };

            return respuesta;
        }

        //método para retornar una respuesta de error
        private AuthResponseDTO BuildErrorResponse(string errorCode, string errorDescription)
        {
            //Si hay un error lo obtenemos y devolvemos su código de error y su descripción
            return new AuthResponseDTO
            {
                Error = new ApiError
                {
                    Code = errorCode,
                    Description = errorDescription,
                    StatusCode = 400
                }
            };
        }

        public async Task<ApplicationUserDTO> Patch(Guid id,
            JsonPatchDocument<ApplicationUserPatchDTO> patchDocument,
            ModelStateDictionary modelState)
        {

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Id == id.ToString());

            if (user == null)
            {
                return null;
            }

            var applicationUserPatchDTO = _mapper.Map<ApplicationUserPatchDTO>(user);

            patchDocument.ApplyTo(applicationUserPatchDTO, modelState);

            if (!modelState.IsValid)
            {
                throw new BadHttpRequestException("The modelstate is not valid");
            }

            _mapper.Map(applicationUserPatchDTO, user);

            await _context.SaveChangesAsync();
            var applicationUserDTO = _mapper.Map<ApplicationUserDTO>(user);
            return applicationUserDTO;
        }

        public async Task<ApplicationUserDTO> ChangeUserStatus(Guid id)
        {

            var user = await _context.ApplicationUsers.FindAsync(id.ToString());

            if (user == null) return null;

            await _context.SaveChangesAsync();
            var dto = _mapper.Map<ApplicationUserDTO>(user);
            return dto;

        }

        public async Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal principal)
        {

            var id = principal.Claims.FirstOrDefault(c => c.Type == CustomClaims.ID_CLAIM_TYPE)?.Value;
            if (id == null) throw new ArgumentNullException(nameof(id));

            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(au => au.Id == id);
            if (user == null) throw new ArgumentNullException(nameof(user));

            return user;
        }
    }
}
