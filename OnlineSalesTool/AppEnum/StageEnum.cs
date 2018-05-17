namespace OnlineSalesTool.AppEnum
{
    /// <summary>
    /// Must be in sync with db
    /// </summary>
    public enum Stage
    {
        NotAssigned, //Newly received cases & not assign or unable to assign to CA
        CustomerConfirm, //CA confirm customer to next stage or Reject case
        EnterContractNumber, //Input indus contract number to start tracking for final status
        WaitForFinalStatus, //Tracking for final status
        WaitForDealerNumber, //Waiting for dealer to generate online order number
        Approved, //Final INDUS status
        Reject, //All reason to reject go here
        NotAssignable //No CA to assign
    }
}
