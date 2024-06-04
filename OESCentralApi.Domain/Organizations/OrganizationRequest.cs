using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OESCentralApi.Domain.Organizations;
public record OrganizationRequest(string Name, Uri Uri);
public record OrganizationPasswordRequest(Uri Uri, string Password);
