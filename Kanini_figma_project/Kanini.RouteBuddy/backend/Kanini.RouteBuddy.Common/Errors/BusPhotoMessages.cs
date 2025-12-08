namespace Kanini.RouteBuddy.Common.Errors;

public static class BusPhotoMessages
{
    public static class ErrorCodes
    {
        public const string BusPhotoNotFound = "BusPhoto.NotFound";
        public const string BusPhotoCreateFailed = "BusPhoto.CreateFailed";
        public const string BusPhotoGetFailed = "BusPhoto.GetFailed";
        public const string BusPhotoGetByBusFailed = "BusPhoto.GetByBusFailed";
        public const string BusPhotoUpdateFailed = "BusPhoto.UpdateFailed";
        public const string BusPhotoDeleteFailed = "BusPhoto.DeleteFailed";
        public const string BusPhotoExistsFailed = "BusPhoto.ExistsFailed";
        public const string BusPhotoCountFailed = "BusPhoto.CountFailed";
        public const string BusPhotoUnexpectedError = "BusPhoto.UnexpectedError";
        public const string BusPhotoLimitExceeded = "BusPhoto.LimitExceeded";
        public const string BusPhotoInvalidFile = "BusPhoto.InvalidFile";
        public const string BusPhotoFileTooLarge = "BusPhoto.FileTooLarge";
        public const string BusPhotoInvalidContent = "BusPhoto.InvalidContent";
    }

    public static class ErrorMessages
    {
        public const string BusPhotoNotFound = "Bus photo not found";
        public const string BusPhotoCreateFailed = "Failed to create bus photo";
        public const string BusPhotoUpdateFailed = "Failed to update bus photo";
        public const string BusPhotoDeleteFailed = "Failed to delete bus photo";
        public const string UnexpectedError = "An unexpected error occurred while processing bus photo";
        public const string DatabaseError = "Database operation failed";
        public const string BusPhotoLimitExceeded = "Maximum number of photos per bus exceeded";
        public const string InvalidFile = "Invalid photo file";
        public const string FileTooLarge = "Photo file size exceeds maximum limit";
        public const string InvalidContent = "Invalid photo file content";
    }

    public static class LogMessages
    {
        public const string BusPhotoCreationStarted = "Bus photo creation started for bus ID: {BusId}";
        public const string BusPhotoCreatedSuccessfully = "Bus photo created successfully with ID: {PhotoId}";
        public const string BusPhotoCreationFailed = "Bus photo creation failed: {Error}";
        public const string BusPhotoRetrievalStarted = "Bus photo retrieval started for ID: {PhotoId}";
        public const string BusPhotoRetrievedSuccessfully = "Bus photo retrieved successfully: {PhotoId}";
        public const string BusPhotoRetrievalFailed = "Bus photo retrieval failed: {Error}";
        public const string BusPhotoUpdateStarted = "Bus photo update started for ID: {PhotoId}";
        public const string BusPhotoUpdatedSuccessfully = "Bus photo updated successfully: {PhotoId}";
        public const string BusPhotoUpdateFailed = "Bus photo update failed: {Error}";
        public const string BusPhotoDeleteStarted = "Bus photo deletion started for ID: {PhotoId}";
        public const string BusPhotoDeletedSuccessfully = "Bus photo deleted successfully: {PhotoId}";
        public const string BusPhotoDeleteFailed = "Bus photo deletion failed: {Error}";
        public const string BusPhotosRetrievalStarted = "Bus photos retrieval started for bus ID: {BusId}";
        public const string BusPhotosRetrievedSuccessfully = "Retrieved {Count} photos for bus ID: {BusId}";
    }
}