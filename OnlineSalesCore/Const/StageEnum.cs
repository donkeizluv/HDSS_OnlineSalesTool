namespace OnlineSalesCore.Const
{
    /// <summary>
    /// Must be in sync of order with db ProcessStage
    /// </summary>
    public enum StageEnum
    {
        NotAssigned, //Newly received cases
        CustomerConfirm, //CA confirm customer to next stage or Reject case
        EnterContractNumber, //Input indus contract number to start tracking for final status
        WaitForFinalStatus, //Tracking for final status
        WaitForOnlineBill, //Waiting for dealer to generate online order number
        WaitForDocument, //CA checks if documents customer provide legit
        Reject, //All reason to reject go here
        CustomerReject,
        NotAssignable, //No CA to assign
        Completed //Done case
    }
}
