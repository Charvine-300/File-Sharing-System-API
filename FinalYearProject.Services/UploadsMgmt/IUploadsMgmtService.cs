using FinalYearProject.Data.Utilities;

namespace FinalYearProject.Services.UploadsMgmt;

public interface IUploadsMgmtService
{
    Task<ServiceResponse<PaginationResponse<FileResponse>>> GetFilesAsync(
        FileParameters parameters,
        CancellationToken cancellationToken);

    Task<ServiceResponse<FileDetailsResponse>> GetFileAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ServiceResponse> UploadFileAsync(
        FileMgmtRequest request,
        CancellationToken cancellationToken);

    Task<ServiceResponse<FileDownloadResponse>> DownloadFileAsync(
        Guid fileId,
        CancellationToken cancellationToken);

    Task<ServiceResponse> UpdateFilePolicyAsync(
        Guid id,
        UpdateFilePolicyRequest request,
        CancellationToken cancellationToken);

    Task<ServiceResponse> DeleteFileAsync(
       Guid id,
       CancellationToken cancellationToken);



}
