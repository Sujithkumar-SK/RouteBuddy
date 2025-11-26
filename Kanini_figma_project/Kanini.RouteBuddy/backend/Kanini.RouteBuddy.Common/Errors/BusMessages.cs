namespace Kanini.RouteBuddy.Common.Errors;

public static class BusMessages
{
    public static class LogMessages
    {
        public const string BusCreationStarted = "Starting bus creation: {BusName}";
        public const string CreatingBus = "Creating bus: {BusName}";
        public const string BusCreatedSuccessfully = "Bus created successfully: {BusId}";
        public const string BusCreationFailed = "Bus creation failed for: {BusName}";
        public const string BusCreationException = "Exception occurred while creating bus: {BusName}";
        public const string RegistrationExistsWarning = "Registration number already exists: {RegistrationNumber}";
        public const string BusRetrievalStarted = "Starting bus retrieval: {BusId}";
        public const string GettingBusById = "Getting bus by ID: {BusId}";
        public const string BusRetrievedSuccessfully = "Bus retrieved successfully: {BusId}";
        public const string BusRetrievalFailed = "Bus retrieval failed: {BusId}";
        public const string BusRetrievalException = "Exception occurred while retrieving bus: {BusId}";
        public const string BusNotFound = "Bus not found: {BusId}";
        public const string BusListRetrievalStarted = "Starting bus list retrieval for vendor: {VendorId}";
        public const string GettingBusesByVendor = "Getting buses for vendor: {VendorId}, page: {PageNumber}, size: {PageSize}";
        public const string BusListRetrievedSuccessfully = "Bus list retrieved successfully for vendor: {VendorId}";
        public const string BusesByVendorRetrievedSuccessfully = "Retrieved {Count} buses for vendor: {VendorId}";
        public const string BusListFailed = "Bus list retrieval failed for vendor: {VendorId}";
        public const string BusCountFailed = "Bus count retrieval failed for vendor: {VendorId}";
        public const string BusesByVendorRetrievalFailed = "Failed to retrieve buses for vendor: {VendorId}";
        public const string BusesByVendorException = "Exception occurred while retrieving buses for vendor: {VendorId}";
        public const string BusUpdateStarted = "Starting bus update: {BusId}";
        public const string BusUpdatedSuccessfully = "Bus updated successfully: {BusId}";
        public const string BusUpdateFailed = "Bus update failed: {BusId}";
        public const string BusDeleteStarted = "Starting bus deletion: {BusId}";
        public const string BusDeletedSuccessfully = "Bus deleted successfully: {BusId}";
        public const string BusDeleteFailed = "Bus deletion failed: {BusId}";
        public const string BusActivationStarted = "Starting bus activation: {BusId}";
        public const string ActivatingBus = "Activating bus: {BusId}";
        public const string BusActivatedSuccessfully = "Bus activated successfully: {BusId}";
        public const string BusActivationFailed = "Bus activation failed: {BusId}";
        public const string BusActivationException = "Exception occurred while activating bus: {BusId}";
        public const string UnauthorizedAccessWarning = "Unauthorized access attempt for bus: {BusId}";
        public const string InvalidStatusWarning = "Invalid status for bus operation: {BusId}";
        public const string BusDeactivationStarted = "Starting bus deactivation: {BusId}";
        public const string DeactivatingBus = "Deactivating bus: {BusId}";
        public const string BusDeactivatedSuccessfully = "Bus deactivated successfully: {BusId}";
        public const string BusDeactivationFailed = "Bus deactivation failed: {BusId}";
        public const string BusDeactivationException = "Exception occurred while deactivating bus: {BusId}";
        public const string BusAlreadyInactiveWarning = "Bus is already inactive: {BusId}";
        public const string BusMaintenanceStarted = "Starting bus maintenance setting: {BusId}";
        public const string SettingBusMaintenance = "Setting bus to maintenance: {BusId}";
        public const string BusSetToMaintenanceSuccessfully = "Bus set to maintenance successfully: {BusId}";
        public const string BusMaintenanceSetSuccessfully = "Bus set to maintenance successfully: {BusId}";
        public const string BusMaintenanceFailed = "Bus maintenance setting failed: {BusId}";
        public const string BusMaintenanceException = "Exception occurred while setting bus maintenance: {BusId}";
        public const string AwaitingConfirmationStarted = "Starting awaiting confirmation buses retrieval for vendor: {VendorId}";
        public const string GettingAwaitingConfirmationBuses = "Getting awaiting confirmation buses for vendor: {VendorId}";
        public const string AwaitingConfirmationRetrievedSuccessfully = "Awaiting confirmation buses retrieved successfully for vendor: {VendorId}";
        public const string AwaitingConfirmationBusesRetrievedSuccessfully = "Retrieved {Count} awaiting confirmation buses for vendor: {VendorId}";
        public const string AwaitingConfirmationFailed = "Awaiting confirmation buses retrieval failed for vendor: {VendorId}";
        public const string AwaitingConfirmationBusesRetrievalFailed = "Failed to retrieve awaiting confirmation buses for vendor: {VendorId}";
        public const string AwaitingConfirmationBusesException = "Exception occurred while retrieving awaiting confirmation buses for vendor: {VendorId}";
        
        // Admin specific messages
        public const string GettingAllBuses = "Getting all buses for admin";
        public const string AllBusesRetrievedSuccessfully = "Retrieved {Count} buses for admin";
        public const string AllBusesRetrievalFailed = "Failed to retrieve all buses for admin";
        public const string AllBusesRetrievalException = "Exception occurred while retrieving all buses for admin";
        public const string GettingPendingBuses = "Getting pending buses for admin";
        public const string PendingBusesRetrievedSuccessfully = "Retrieved {Count} pending buses for admin";
        public const string PendingBusesRetrievalFailed = "Failed to retrieve pending buses for admin";
        public const string PendingBusesRetrievalException = "Exception occurred while retrieving pending buses for admin";
        public const string FilteringBuses = "Filtering buses with search: {SearchName}, status: {Status}, active: {IsActive}";
        public const string BusesFilteredSuccessfully = "Filtered {Count} buses successfully";
        public const string BusesFilterFailed = "Failed to filter buses";
        public const string BusesFilterException = "Exception occurred while filtering buses";
        public const string ApprovingBus = "Approving bus: {BusId}";
        public const string BusApprovedSuccessfully = "Bus approved successfully: {BusId}";
        public const string BusApprovalFailed = "Bus approval failed: {BusId}";
        public const string BusApprovalException = "Exception occurred while approving bus: {BusId}";
        public const string RejectingBus = "Rejecting bus: {BusId}";
        public const string BusRejectedSuccessfully = "Bus rejected successfully: {BusId}";
        public const string BusRejectionFailed = "Bus rejection failed: {BusId}";
        public const string BusRejectionException = "Exception occurred while rejecting bus: {BusId}";
        public const string ReactivatingBus = "Reactivating bus: {BusId}";
        public const string BusReactivatedSuccessfully = "Bus reactivated successfully: {BusId}";
        public const string BusReactivationFailed = "Bus reactivation failed: {BusId}";
        public const string BusReactivationException = "Exception occurred while reactivating bus: {BusId}";
        public const string InvalidBusId = "Invalid bus ID: {BusId}";
        public const string InvalidRejectionReason = "Invalid rejection reason for bus: {BusId}";
        public const string InvalidDeactivationReason = "Invalid deactivation reason for bus: {BusId}";
        public const string InvalidReactivationReason = "Invalid reactivation reason for bus: {BusId}";
    }
    
    public static class ErrorMessages
    {
        public const string BusNotFound = "Bus not found";
        public const string InvalidBusData = "Invalid bus data provided";
        public const string RegistrationNumberExists = "Registration number already exists";
        public const string UnauthorizedBusAccess = "Unauthorized access to bus data";
        public const string InvalidBusStatus = "Invalid bus status for this operation";
        public const string BusNotActive = "Bus is not active";
        public const string BusAlreadyInactive = "Bus is already inactive";
        public const string DocumentUploadRequired = "Bus documents are required";
        public const string ValidationError = "Validation error occurred";
        public const string DatabaseError = "Database error occurred";
        public const string UnexpectedError = "An unexpected error occurred";
    }
    
    public static class ErrorCodes
    {
        public const string BusNotFound = "Bus.NotFound";
        public const string BusCreationFailed = "Bus.CreationFailed";
        public const string BusRetrievalFailed = "Bus.RetrievalFailed";
        public const string BusListFailed = "Bus.ListFailed";
        public const string BusCountFailed = "Bus.CountFailed";
        public const string BusUpdateFailed = "Bus.UpdateFailed";
        public const string BusDeleteFailed = "Bus.DeleteFailed";
        public const string BusActivationFailed = "Bus.ActivationFailed";
        public const string BusDeactivationFailed = "Bus.DeactivationFailed";
        public const string BusMaintenanceFailed = "Bus.MaintenanceFailed";
        public const string AwaitingConfirmationFailed = "Bus.AwaitingConfirmationFailed";
        public const string RegistrationExists = "Registration.Exists";
        public const string InvalidStatus = "Bus.InvalidStatus";
        public const string AlreadyInactive = "Bus.AlreadyInactive";
        public const string BusUnexpectedError = "Bus.UnexpectedError";
    }
}