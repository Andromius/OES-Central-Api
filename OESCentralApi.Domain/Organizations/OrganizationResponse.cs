using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OESCentralApi.Domain.Organizations;
public record OrganizationResponse(string Name, Uri Uri);
public record OrganizationPasswordResponse(string Name, Uri Uri, string Password);
