UPDATE [dbo].[BillingRates]
SET [StartDate] = cast([StartDate] as date)
WHERE cast([StartDate] as time) > '00:00'

UPDATE [dbo].[BillingRates]
SET [EndDate] = cast([EndDate] as date)
WHERE cast([EndDate] as time) > '00:00'