SELECT LAD.SZLOANAPPLNNO "ContractNumber"
     , AWD.szssn "NatId"
     , NVL(AWD.SZLNAME, '') || ' ' || NVL(AWD.SZMNAME, '') || ' ' || NVL(AWD.SZFNAME, '') "FullName"
     , lad.fdownpayment "Paid"
     , LAD.FAMTSANCTIONED "LoanAmount"
     , ass.szmakemodeldesc "Product"
     , ass.fprice "Amount"
     ,LAD.Itenor "Term"
FROM
  TR_LOANAPPLICATIONDETAILS@GLM_LOS LAD
, TR_APPLICATIONWORKDETAILS@GLM_LOS AWD
, mst_sublocation@GLM_LOS       msl
, mst_deal@GLM_LOS              md
,tr_asset_sim_details@GLM_LOS ass
, otheraddressdetails@GLM_LOS oad
WHERE 1 = 1
and ass.szparentsimulationno = lad.szleadno
and ass.szsimulationno = lad.szsimulationno  
and LAD.SZLOANAPPLNNO = AWD.SZLOANAPPLNNO
and oad.iaddressseq   = AWD.Iaddressseq
and oad.SZADDRESSTYPE = 'RS'
AND AWD.SZPARTYTYPE = 'BORROWER'
and oad.isrlno = 1
AND msl.szsublocationcode = lad.SZSUBLOCATIONCODE
AND md.szdealcode(+) = lad.szdealcode
and LAD.SZLOANAPPLNNO = '{contract}'