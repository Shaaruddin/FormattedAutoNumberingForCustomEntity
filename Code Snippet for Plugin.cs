

#region Namespaces
using System;
using System.Data;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.Xrm.Sdk;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Runtime.Serialization;
using System.Linq;
#endregion


[Microsoft.SqlServer.Dts.Pipeline.SSISScriptComponentEntryPointAttribute]
public class ScriptMain : UserComponent
{
    IOrganizationService organizationservice;
    

    public override void PreExecute()
    {
        base.PreExecute();
        ClientCredentials credentials = new ClientCredentials();
        credentials.UserName.UserName = "admin@maxispacrmdemo.onmicrosoft.com";
        credentials.UserName.Password = "Welcome1";
        credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;

        organizationservice = new OrganizationServiceProxy(
            new Uri("https://maxispacrmdemo.api.crm5.dynamics.com/XRMServices/2011/Organization.svc"), null, credentials, null);

        
    }

    
    public override void PostExecute()
    {
        base.PostExecute();
        
    }

   public override void Input0_ProcessInputRow(Input0Buffer Row)
    {
        
        Entity ApprovalProductEnt = new Entity("new_ticketproduct");
        ColumnSet columns = new ColumnSet(true);
        columns = new ColumnSet(new String[] { "new_ticket",
            "new_productgroup",
            "new_producttype",
            "new_productitem" });

        Guid TicketNumberId = new Guid();
        TicketNumberId = getTicketNumber(Row.OppportunityID, ref organizationservice);
        
        //update
        if (TicketNumberId != Guid.Empty)
        {
            ApprovalProductEnt["new_ticket"] = new EntityReference("new_pricingapprovalticket", TicketNumberId);
            
            organizationservice.Update(ApprovalProductEnt);
        }
        else
        //create
        if (TicketNumberId != Guid.Empty)
        {
            ApprovalProductEnt["new_ticket"] = new EntityReference("new_pricingapprovalticket", TicketNumberId);

            organizationservice.Create(ApprovalProductEnt);
        }
        
    }

    public Guid getTicketNumber(string ticketnumber, ref IOrganizationService service)
    {
        Guid TicketNumberGuid = Guid.Empty;
        QueryExpression TicketNumberQuery = new QueryExpression { EntityName = "new_pricingapprovalticket", ColumnSet = new ColumnSet(true) };
        TicketNumberQuery.Criteria.AddCondition("new_ticketnumber", ConditionOperator.Equal, ticketnumber);
        EntityCollection TicketNumberQueryRetrieve = service.RetrieveMultiple(TicketNumberQuery);

        for (var i = 0; i < TicketNumberQueryRetrieve.Entities.Count; i++)
        {
            TicketNumberGuid = TicketNumberQueryRetrieve.Entities[0].GetAttributeValue<Guid>("new_ticket");
        }

        return TicketNumberGuid;
    }
    
}
