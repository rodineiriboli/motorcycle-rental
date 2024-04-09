using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MR.Authentication.Controllers;

[ApiController]
public abstract class MainController : Controller
{
    protected ICollection<string> Erros = new List<string>();
    protected ActionResult CustomResponse(object result = null)
    {
        if (ValidOption())
        {
            return Ok(result);
        }

        return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                {"Mensagens", Erros.ToArray() }
            }));
    }

    protected ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        var erros = modelState.Values.SelectMany(e => e.Errors);

        foreach (var erro in erros)
        {
            AddProcessError(erro.ErrorMessage);
        }

        return CustomResponse();
    }

    protected bool ValidOption()
    {
        return !Erros.Any();
    }

    protected void AddProcessError(string erro)
    {
        Erros.Add(erro);
    }

    protected void ClearProcessError()
    {
        Erros.Clear();
    }
}
