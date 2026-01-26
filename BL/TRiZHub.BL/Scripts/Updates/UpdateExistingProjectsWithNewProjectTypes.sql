USE [TRiZHub]
GO
-- Flex
-- Update Projects
UPDATE Project SET ProjectTypeId = (SELECT Id From ProjectType Where Name = 'Flex Engineering') where Id IN('4929d261-7299-4d88-ab8f-8882b7647860')
-- Update Sub Projects
update SubProject SET SubProjectTypeId = (SELECT Id From ProjectType Where Name = 'Flex Engineering') where Id IN(SELECT sp.id as SubProjectId FROM Project p
left join SubProject sp on sp.ProjectId = p.Id
WHERE p.Id IN('4929d261-7299-4d88-ab8f-8882b7647860'))

-- Admin
-- Update Projects
UPDATE Project SET ProjectTypeId = (SELECT Id From ProjectType Where Name = 'Admin') where Id IN('2fb5d095-dabe-46a3-b1e0-78f6a68f313f', '0b741e73-8b8c-4fe2-a113-7b203d7adddc', 'c8a4a1e6-1796-4579-ad4e-d4d2bf2389bc', '7c4e242c-0f61-41af-bcc0-2f7623cacd27', '9f63c807-2d4d-4a9a-9051-a091971fc182', '41d9994b-d387-4704-aad7-c738cd26484c', '8c6b0b36-2734-4833-a0b3-16ac83233019', 'b4295daf-47a7-4f25-8dea-0a563d692360', '3cfe9517-06a2-48d2-b713-978efbe71713')
-- Update Sub Projects
update SubProject SET SubProjectTypeId = (SELECT Id From ProjectType Where Name = 'Admin') where Id IN(SELECT sp.id as SubProjectId FROM Project p
left join SubProject sp on sp.ProjectId = p.Id
WHERE p.Id IN('2fb5d095-dabe-46a3-b1e0-78f6a68f313f', '0b741e73-8b8c-4fe2-a113-7b203d7adddc', 'c8a4a1e6-1796-4579-ad4e-d4d2bf2389bc', '7c4e242c-0f61-41af-bcc0-2f7623cacd27', '9f63c807-2d4d-4a9a-9051-a091971fc182', '41d9994b-d387-4704-aad7-c738cd26484c', '8c6b0b36-2734-4833-a0b3-16ac83233019', 'b4295daf-47a7-4f25-8dea-0a563d692360', '3cfe9517-06a2-48d2-b713-978efbe71713'))

-- System
-- Update Projects
UPDATE Project SET ProjectTypeId = (SELECT Id From ProjectType Where Name = 'System') where Id IN('501bf81c-5750-4de9-a94e-0bc05918b95e', '6bb891b3-ae9c-46d0-9379-d86fec04e301', '1eeaa9ce-97fa-447a-91a8-85b3e09efd8d', '037094fc-54ca-44db-8c99-05e5c3ec1499', '74bd24f1-0186-4174-a092-f58f706d508c')
-- Update Sub Projects
update SubProject SET SubProjectTypeId = (SELECT Id From ProjectType Where Name = 'System') where Id IN(SELECT sp.id as SubProjectId FROM Project p
left join SubProject sp on sp.ProjectId = p.Id
WHERE p.Id IN('501bf81c-5750-4de9-a94e-0bc05918b95e', '6bb891b3-ae9c-46d0-9379-d86fec04e301', '1eeaa9ce-97fa-447a-91a8-85b3e09efd8d', '037094fc-54ca-44db-8c99-05e5c3ec1499', '74bd24f1-0186-4174-a092-f58f706d508c'))

-- Training
-- Update Projects
UPDATE Project SET ProjectTypeId = (SELECT Id From ProjectType Where Name = 'Training') where Id IN('853d9dcd-f599-4bdb-9c8e-1e37625c5b59', 'c4f23012-bc33-4390-9b42-f16e130f4963')
-- Update Sub Projects
update SubProject SET SubProjectTypeId = (SELECT Id From ProjectType Where Name = 'Training') where Id IN(SELECT sp.id as SubProjectId FROM Project p
left join SubProject sp on sp.ProjectId = p.Id
WHERE p.Id IN('853d9dcd-f599-4bdb-9c8e-1e37625c5b59', 'c4f23012-bc33-4390-9b42-f16e130f4963'))

-- Leave Vacation
-- Update Projects
UPDATE Project SET ProjectTypeId = (SELECT Id From ProjectType Where Name = 'Leave (Vacation)') where Id IN('47c834e1-a975-49d8-bbc1-cf784feac0fc', 'ed99f6e0-e6a8-45b1-8516-bcd32a13c734')
-- Update Sub Projects
update SubProject SET SubProjectTypeId = (SELECT Id From ProjectType Where Name = 'Leave (Vacation)') where Id IN(SELECT sp.id as SubProjectId FROM Project p
left join SubProject sp on sp.ProjectId = p.Id
WHERE p.Id IN('47c834e1-a975-49d8-bbc1-cf784feac0fc', 'ed99f6e0-e6a8-45b1-8516-bcd32a13c734'))

-- Leave Sick
-- Update Projects
UPDATE Project SET ProjectTypeId = (SELECT Id From ProjectType Where Name = 'Leave (Sick)') where Id IN('c964c3c0-ab8e-4778-947a-2a0e08c86c48')
-- Update Sub Projects
update SubProject SET SubProjectTypeId = (SELECT Id From ProjectType Where Name = 'Leave (Sick)') where Id IN(SELECT sp.id as SubProjectId FROM Project p
left join SubProject sp on sp.ProjectId = p.Id
WHERE p.Id IN('c964c3c0-ab8e-4778-947a-2a0e08c86c48'))

-- Leave Study
-- Update Projects
UPDATE Project SET ProjectTypeId = (SELECT Id From ProjectType Where Name = 'Leave (Study)') where Id IN('25123913-7692-49d5-9249-f671ae42448d')
-- Update Sub Projects
update SubProject SET SubProjectTypeId = (SELECT Id From ProjectType Where Name = 'Leave (Study)') where Id IN(SELECT sp.id as SubProjectId FROM Project p
left join SubProject sp on sp.ProjectId = p.Id
WHERE p.Id IN('25123913-7692-49d5-9249-f671ae42448d'))

-- Non Eligible
-- Update Projects
UPDATE Project SET ProjectTypeId = (SELECT Id From ProjectType Where Name = 'Non-Eligible') where Id IN('dac5feec-d943-487e-a915-072d1a1b6df3', 'c8ce29d9-6526-4637-bc70-5f019f274f38', '329250ae-b4e8-4cb9-a2cb-165936a5baed')
-- Update Sub Projects
update SubProject SET SubProjectTypeId = (SELECT Id From ProjectType Where Name = 'Non-Eligible') where Id IN(SELECT sp.id as SubProjectId FROM Project p
left join SubProject sp on sp.ProjectId = p.Id
WHERE p.Id IN('dac5feec-d943-487e-a915-072d1a1b6df3', 'c8ce29d9-6526-4637-bc70-5f019f274f38', '329250ae-b4e8-4cb9-a2cb-165936a5baed'))




