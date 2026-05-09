using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    public class DocumentsModel : PageModel
    {
        private readonly IDocumentService _documentService;
        private readonly IDeviceService _deviceService;

        public DocumentsModel(IDocumentService documentService, IDeviceService deviceService)
        {
            _documentService = documentService;
            _deviceService = deviceService;
        }

        public List<DeviceDocument> DocumentList { get; set; } = new List<DeviceDocument>();
        public Dictionary<int, string> DeviceNames { get; set; } = new Dictionary<int, string>();
        public DocumentType? TypeFilter { get; set; }

        public async Task OnGetAsync(DocumentType? type = null)
        {
            TypeFilter = type;

            var allDocuments = await _documentService.GetAllDocumentsAsync();
            var allDevices = await _deviceService.GetAllDevicesAsync() ?? new List<MedicalDevice>();
            
            DeviceNames = allDevices
                .Where(d => d != null && !string.IsNullOrEmpty(d.DeviceName))
                .ToDictionary(d => d.Id, d => d.DeviceName);

            var filtered = allDocuments.AsQueryable();

            if (type.HasValue)
            {
                filtered = filtered.Where(d => d.DocumentType == type.Value).AsQueryable();
            }

            DocumentList = filtered.OrderByDescending(d => d.UploadDate).ToList();
        }
    }
}
