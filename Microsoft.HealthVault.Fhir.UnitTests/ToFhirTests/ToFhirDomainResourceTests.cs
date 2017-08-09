using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class ToFhirDomainResourceTests
    {

        [TestMethod]
        public void WhenHealthvaultThingIsTransformedToFhirDomainResource_ThenNotesAreTransformedtoNarrative()
        {
            var noteData = "Some Random Notes";
            ThingBase thing = new Height(34.4);
            thing.CommonData.Note = noteData;

            DomainResource domainResource = thing.ToFhir() as DomainResource;

            Assert.AreEqual(noteData, domainResource.Text.Div);
        }
    }
}
