using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace WebAPI.Identity.Dto
{
    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string NomeCompleto { get; set; }
    }
}
