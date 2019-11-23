using PALMS.Data.Objects.ClientModel;
using PALMS.ViewModels.EntityViewModel;

namespace PALMS.ViewModels.Common
{
    public static class HasChangesHelper
    {
        
        public static bool HasChanges(this ClientViewModel client, bool deep = true)
        {
            var originalObject = client.OriginalObject;

            return originalObject.IsNew ||
                !Equals(originalObject.Name, client.Name) ||
                   !Equals(originalObject.ShortName, client.ShortName) ||
                originalObject.Active != client.Active ||
                
                (!deep ||
                
                (client.ClientInfo.HasChanges() || client.InvoiceDetail.HasChanges()));
        }
    }

}
