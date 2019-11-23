using System.Data.Entity;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.LinenModel;

namespace PALMS.Data.Services
{
    public class ContextInitializer : CreateDatabaseIfNotExists<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            //AddFamilyLinen(context);
            context.SaveChanges();
        }

        private void AddFamilyLinen(DataContext context)
        {
            var familyLinen = context.FamilyLinens;

            //familyLinen.Add(new FamilyLinen { Name = "Shirt" });

            familyLinen.Add(new FamilyLinen { Name = "ABAYA" });
            familyLinen.Add(new FamilyLinen { Name = "APPLICATOR" });
            familyLinen.Add(new FamilyLinen { Name = "APRON" });
            familyLinen.Add(new FamilyLinen { Name = "ARM REST" });
            familyLinen.Add(new FamilyLinen { Name = "BACK COVER" });
            familyLinen.Add(new FamilyLinen { Name = "BACK REST" });
            familyLinen.Add(new FamilyLinen { Name = "BACK SUPPORT" });
            familyLinen.Add(new FamilyLinen { Name = "BAG" });
            familyLinen.Add(new FamilyLinen { Name = "BANDANA" });
            familyLinen.Add(new FamilyLinen { Name = "BATH MAT" });
            familyLinen.Add(new FamilyLinen { Name = "BATH ROBE" });
            familyLinen.Add(new FamilyLinen { Name = "BATH RUG" });
            familyLinen.Add(new FamilyLinen { Name = "BATH SHEET " });
            familyLinen.Add(new FamilyLinen { Name = "BATH TOWEL" });
            familyLinen.Add(new FamilyLinen { Name = "BED" });
            familyLinen.Add(new FamilyLinen { Name = "BED COVER" });
            familyLinen.Add(new FamilyLinen { Name = "BED LINER" });
            familyLinen.Add(new FamilyLinen { Name = "BED RUNNER" });
            familyLinen.Add(new FamilyLinen { Name = "BED SHEET" });
            familyLinen.Add(new FamilyLinen { Name = "BED SPREAD" });
            familyLinen.Add(new FamilyLinen { Name = "BED THROW " });
            familyLinen.Add(new FamilyLinen { Name = "BEDDING" });
            familyLinen.Add(new FamilyLinen { Name = "BELT" });
            familyLinen.Add(new FamilyLinen { Name = "BIDET TOWEL" });
            familyLinen.Add(new FamilyLinen { Name = "BLANKET" });
            familyLinen.Add(new FamilyLinen { Name = "BLOUSE" });
            familyLinen.Add(new FamilyLinen { Name = "BOTTOM SHEET" });
            familyLinen.Add(new FamilyLinen { Name = "BOW TIE" });
            familyLinen.Add(new FamilyLinen { Name = "BRASSIERE" });
            familyLinen.Add(new FamilyLinen { Name = "BREAD BASKET" });
            familyLinen.Add(new FamilyLinen { Name = "BREAD COVER" });
            familyLinen.Add(new FamilyLinen { Name = "CAP" });
            familyLinen.Add(new FamilyLinen { Name = "CARPET" });
            familyLinen.Add(new FamilyLinen { Name = "CHAIR" });
            familyLinen.Add(new FamilyLinen { Name = "CHEST COVER" });
            familyLinen.Add(new FamilyLinen { Name = "CHEST SUPPORT" });
            familyLinen.Add(new FamilyLinen { Name = "CLOTH" });
            familyLinen.Add(new FamilyLinen { Name = "COASTER" });
            familyLinen.Add(new FamilyLinen { Name = "COAT" });
            familyLinen.Add(new FamilyLinen { Name = "COMFORTER" });
            familyLinen.Add(new FamilyLinen { Name = "COSTUME" });
            familyLinen.Add(new FamilyLinen { Name = "COVERALL" });
            familyLinen.Add(new FamilyLinen { Name = "CURTAIN" });
            familyLinen.Add(new FamilyLinen { Name = "CUSHION" });
            familyLinen.Add(new FamilyLinen { Name = "DRESS" });
            familyLinen.Add(new FamilyLinen { Name = "DUSTER" });
            familyLinen.Add(new FamilyLinen { Name = "DUVET" });
            familyLinen.Add(new FamilyLinen { Name = "DUVET COVER" });
            familyLinen.Add(new FamilyLinen { Name = "EQUIPMENT COVER" });
            familyLinen.Add(new FamilyLinen { Name = "FACE MASK" });
            familyLinen.Add(new FamilyLinen { Name = "FACE TOWEL" });
            familyLinen.Add(new FamilyLinen { Name = "FITTED SHEET" });
            familyLinen.Add(new FamilyLinen { Name = "FLAG" });
            familyLinen.Add(new FamilyLinen { Name = "FLAT LINENS" });
            familyLinen.Add(new FamilyLinen { Name = "FOAM BACK" });
            familyLinen.Add(new FamilyLinen { Name = "FOAM COVER" });
            familyLinen.Add(new FamilyLinen { Name = "FOOT COVER" });
            familyLinen.Add(new FamilyLinen { Name = "FOOT SUPPORTER" });
            familyLinen.Add(new FamilyLinen { Name = "FWALA COVER" });
            familyLinen.Add(new FamilyLinen { Name = "GLASS BRUSH" });
            familyLinen.Add(new FamilyLinen { Name = "GLOVES" });
            familyLinen.Add(new FamilyLinen { Name = "GOWN" });
            familyLinen.Add(new FamilyLinen { Name = "GUTRA" });
            familyLinen.Add(new FamilyLinen { Name = "GUTRA & KHAFIA" });
            familyLinen.Add(new FamilyLinen { Name = "HAND BAND" });
            familyLinen.Add(new FamilyLinen { Name = "HAND MITTS" });
            familyLinen.Add(new FamilyLinen { Name = "HAND TOWEL" });
            familyLinen.Add(new FamilyLinen { Name = "HANDKERCHEIF" });
            familyLinen.Add(new FamilyLinen { Name = "HEAD BAND" });
            familyLinen.Add(new FamilyLinen { Name = "HEAD COVER" });
            familyLinen.Add(new FamilyLinen { Name = "HEAD FOAM" });
            familyLinen.Add(new FamilyLinen { Name = "HEAD REST" });
            familyLinen.Add(new FamilyLinen { Name = "HEAD SUPPORT" });
            familyLinen.Add(new FamilyLinen { Name = "HIP COVER" });
            familyLinen.Add(new FamilyLinen { Name = "HIP STRAP" });
            familyLinen.Add(new FamilyLinen { Name = "JACKET" });
            familyLinen.Add(new FamilyLinen { Name = "KAMIZ" });
            familyLinen.Add(new FamilyLinen { Name = "KANDURA" });
            familyLinen.Add(new FamilyLinen { Name = "KIMONO" });
            familyLinen.Add(new FamilyLinen { Name = "KNEE COVER" });
            familyLinen.Add(new FamilyLinen { Name = "KNEE PAD" });
            familyLinen.Add(new FamilyLinen { Name = "KURTA" });
            familyLinen.Add(new FamilyLinen { Name = "LEGGING" });
            familyLinen.Add(new FamilyLinen { Name = "LUNGI" });
            familyLinen.Add(new FamilyLinen { Name = "MAGAZINE HOLDER" });
            familyLinen.Add(new FamilyLinen { Name = "MASSAGE SHEET" });
            familyLinen.Add(new FamilyLinen { Name = "MAT" });
            familyLinen.Add(new FamilyLinen { Name = "MATTRESSS" });
            familyLinen.Add(new FamilyLinen { Name = "MOLTEN" });
            familyLinen.Add(new FamilyLinen { Name = "MOP" });
            familyLinen.Add(new FamilyLinen { Name = "MOTON" });
            familyLinen.Add(new FamilyLinen { Name = "NAPKIN" });
            familyLinen.Add(new FamilyLinen { Name = "OUTERWEAR" });
            familyLinen.Add(new FamilyLinen { Name = "OVER LACE" });
            familyLinen.Add(new FamilyLinen { Name = "PAJAMA" });
            familyLinen.Add(new FamilyLinen { Name = "PANTIES" });
            familyLinen.Add(new FamilyLinen { Name = "PANTS" });
            familyLinen.Add(new FamilyLinen { Name = "PEDIATRIC TOY" });
            familyLinen.Add(new FamilyLinen { Name = "PILLOW" });
            familyLinen.Add(new FamilyLinen { Name = "PILLOW CASE" });
            familyLinen.Add(new FamilyLinen { Name = "POOL TOWEL" });
            familyLinen.Add(new FamilyLinen { Name = "PULLOVER" });
            familyLinen.Add(new FamilyLinen { Name = "RIBBON" });
            familyLinen.Add(new FamilyLinen { Name = "SALWAR" });
            familyLinen.Add(new FamilyLinen { Name = "SALWAR KAMIZ " });
            familyLinen.Add(new FamilyLinen { Name = "SAREE" });
            familyLinen.Add(new FamilyLinen { Name = "SCAF FOAM" });
            familyLinen.Add(new FamilyLinen { Name = "SCARF" });
            familyLinen.Add(new FamilyLinen { Name = "SEAT COVER" });
            familyLinen.Add(new FamilyLinen { Name = "SHAWL" });
            familyLinen.Add(new FamilyLinen { Name = "SHEILA" });
            familyLinen.Add(new FamilyLinen { Name = "SHIRT" });
            familyLinen.Add(new FamilyLinen { Name = "SHOES PAIR" });
            familyLinen.Add(new FamilyLinen { Name = "SHORT" });
            familyLinen.Add(new FamilyLinen { Name = "SIDE HEAD REST" });
            familyLinen.Add(new FamilyLinen { Name = "SIDE RAIL" });
            familyLinen.Add(new FamilyLinen { Name = "SIDE RAIL FOAM" });
            familyLinen.Add(new FamilyLinen { Name = "SKIRT" });
            familyLinen.Add(new FamilyLinen { Name = "SLACKS" });
            familyLinen.Add(new FamilyLinen { Name = "SLEEPING PLASTIC COVER" });
            familyLinen.Add(new FamilyLinen { Name = "SLEEVELESS" });
            familyLinen.Add(new FamilyLinen { Name = "SLIDING SHEET" });
            familyLinen.Add(new FamilyLinen { Name = "SLIP" });
            familyLinen.Add(new FamilyLinen { Name = "SLIP/STOCKING" });
            familyLinen.Add(new FamilyLinen { Name = "SOCKS" });
            familyLinen.Add(new FamilyLinen { Name = "SOFA COVER " });
            familyLinen.Add(new FamilyLinen { Name = "SOFA PROTECTOR" });
            familyLinen.Add(new FamilyLinen { Name = "SPA FACE COVER" });
            familyLinen.Add(new FamilyLinen { Name = "SPA FACE CRADLE" });
            familyLinen.Add(new FamilyLinen { Name = "SPA MITTS" });
            familyLinen.Add(new FamilyLinen { Name = "SPANDEX" });
            familyLinen.Add(new FamilyLinen { Name = "SUIT" });
            familyLinen.Add(new FamilyLinen { Name = "SWEATER" });
            familyLinen.Add(new FamilyLinen { Name = "TABLE CLOTH" });
            familyLinen.Add(new FamilyLinen { Name = "TABLE COVER" });
            familyLinen.Add(new FamilyLinen { Name = "TABLE RUNNER" });
            familyLinen.Add(new FamilyLinen { Name = "TABLE SKIRTING" });
            familyLinen.Add(new FamilyLinen { Name = "TENT COVER" });
            familyLinen.Add(new FamilyLinen { Name = "TIE" });
            familyLinen.Add(new FamilyLinen { Name = "TOP SHEET" });
            familyLinen.Add(new FamilyLinen { Name = "TOWEL" });
            familyLinen.Add(new FamilyLinen { Name = "TOYS" });
            familyLinen.Add(new FamilyLinen { Name = "TRAY COVER " });
            familyLinen.Add(new FamilyLinen { Name = "TRAY LINER" });
            familyLinen.Add(new FamilyLinen { Name = "TROLLEY" });
            familyLinen.Add(new FamilyLinen { Name = "TROUSER" });
            familyLinen.Add(new FamilyLinen { Name = "T-SHIRT" });
            familyLinen.Add(new FamilyLinen { Name = "UMBRELLA" });
            familyLinen.Add(new FamilyLinen { Name = "UNDERGARMENT" });
            familyLinen.Add(new FamilyLinen { Name = "UNDERPANTS" });
            familyLinen.Add(new FamilyLinen { Name = "UNDERSHIRT" });
            familyLinen.Add(new FamilyLinen { Name = "UNDERSHORT" });
            familyLinen.Add(new FamilyLinen { Name = "UNDERSLIP" });
            familyLinen.Add(new FamilyLinen { Name = "UNDERWEAR" });
            familyLinen.Add(new FamilyLinen { Name = "VEST" });
            familyLinen.Add(new FamilyLinen { Name = "WASH MITT" });
            familyLinen.Add(new FamilyLinen { Name = "WIPER PAD" });
            familyLinen.Add(new FamilyLinen { Name = "WOOL WEAR" });
        }

        private void AddPrimeInfo(DataContext contex)
        {
            var primeInfo = contex.PrimeInfos;

            primeInfo.Add(new PrimeInfo
            {
                Name = "Laundristic",
                Address = "Abu Dhabi",
            });
        }

    }
}