# DevOps Work Items Mapping

This directory contains the mapping between Business Tasks (from the wiki) and Azure DevOps work items.

## Files

- `devops_tasks.json` - JSON format mapping file
- `devops_tasks.csv` - CSV format mapping file

Both files contain the same data in different formats. The AI assistant will read whichever format is available.

## Structure

### JSON Format

```json
{
  "version": "1.0",
  "exportDate": "2024-01-01T00:00:00Z",
  "tasks": [
    {
      "taskName": "Tasks: {Task Name}",
      "workitemid": "123",
      "workitemUrl": "https://dev.azure.com/.../_workitems/edit/123",
      "useCase": "User Story: {Use Case Name}",
      "process": "{Process Name}"
    }
  ]
}
```

### CSV Format

```csv
Task Name,Work Item ID,Work Item URL,Use Case,Process
"Tasks: {Task Name}",123,https://dev.azure.com/.../_workitems/edit/123,"User Story: {Use Case Name}","{Process Name}"
```

## Exporting from Azure DevOps

### Method 1: Using Azure DevOps REST API

1. Get your Personal Access Token (PAT) with Work Items (read) scope
2. Use the Azure DevOps REST API to export work items:

```bash
curl -u :{PAT} "https://dev.azure.com/{organization}/{project}/_apis/wit/wiql?api-version=7.0" \
  -H "Content-Type: application/json" \
  -d '{"query": "SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.WorkItemType] = '\''Task'\'' AND [System.AreaPath] = '\''Student Course Enrollment System'\''"}'
```

### Method 2: Manual Export

1. Go to Azure DevOps → Boards → Queries
2. Create a query for all Tasks in "Student Course Enrollment System"
3. Export to CSV
4. Map the columns to match the expected format:
   - `System.Title` → `Task Name` (prefix with "Tasks: " if needed)
   - `System.Id` → `Work Item ID`
   - Construct `Work Item URL` from organization, project, and ID
   - Extract `Use Case` from related User Story
   - Extract `Process` from Area Path

### Method 3: Using Azure DevOps CLI

```bash
az boards work-item query --wiql "SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.WorkItemType] = 'Task'" --output json > devops_tasks.json
```

## Mapping Rules

The AI assistant uses the following precedence for mapping:

1. **Exact title match**: Match "Tasks: {Task Name}" exactly
2. **Slug or filename match**: Match based on filename or slug
3. **Fuzzy match**: Match using Use Case + Task content similarity

## Updating the Mapping

When new work items are created in Azure DevOps:

1. Export the updated work items
2. Update either `devops_tasks.json` or `devops_tasks.csv`
3. Ensure the `Task Name` matches exactly with the wiki task heading: "Tasks: {Task Name}"
4. Include the full `workitemUrl` for traceability

## Usage

The AI assistant automatically reads this file during:
- Rule discovery
- Task execution planning
- Branch naming
- Commit message generation
- Pull Request creation

---




