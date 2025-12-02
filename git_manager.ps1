# Git Manager Tool
# Encoding: UTF-8

$Host.UI.RawUI.WindowTitle = "Git Manager"

function Write-Color {
    param($Text, $Color = "White")
    Write-Host $Text -ForegroundColor $Color
}

function Show-Menu {
    Clear-Host
    Write-Color "================================" "Yellow"
    Write-Color "      Git Manager Tool" "Yellow"
    Write-Color "================================" "Yellow"
    Write-Host ""
    Write-Host "1. Commit and Push"
    Write-Host "2. Push Only"
    Write-Host "3. Pull Specific Version"
    Write-Host "4. Show Git Status"
    Write-Host "5. Exit"
    Write-Host ""
}

function Test-GitRepo {
    $null = git rev-parse --git-dir 2>&1
    return $LASTEXITCODE -eq 0
}

function Do-CommitPush {
    Write-Color "`n========== Commit and Push ==========" "Cyan"
    
    Write-Color "`nCurrent changes:" "Yellow"
    git status --short
    
    Write-Host "`nDetailed status:"
    git status
    
    $continue = Read-Host "`nContinue with commit? (Y/N)"
    if ($continue -ne 'Y' -and $continue -ne 'y') {
        Write-Color "Operation cancelled" "Yellow"
        return
    }
    
    # Input commit message FIRST
    $commitMsg = Read-Host "`nEnter commit message"
    if ([string]::IsNullOrWhiteSpace($commitMsg)) {
        $commitMsg = "Update: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
        Write-Color "Using default message: $commitMsg" "Yellow"
    }
    
    # Get list of changed files
    Write-Color "`nSelecting files to add..." "Yellow"
    $changedFiles = git status --porcelain
    
    if (-not $changedFiles) {
        Write-Color "No changes to commit!" "Yellow"
        return
    }
    
    $filesToAdd = @()
    $lines = $changedFiles -split "`n"
    
    foreach ($line in $lines) {
        if ([string]::IsNullOrWhiteSpace($line)) { continue }
        
        # Parse git status output
        $status = $line.Substring(0, 2)
        $file = $line.Substring(3).Trim()
        
        # Show file status
        $statusText = switch ($status.Trim()) {
            "M" { "Modified" }
            "A" { "Added" }
            "D" { "Deleted" }
            "R" { "Renamed" }
            "C" { "Copied" }
            "U" { "Updated" }
            "??" { "Untracked" }
            default { $status }
        }
        
        Write-Host ""
        Write-Color "File: $file" "Cyan"
        Write-Color "Status: $statusText" "Yellow"
        
        $addFile = Read-Host "Add this file? (Y/N/A=Add all remaining)"
        
        if ($addFile -eq 'A' -or $addFile -eq 'a') {
            # Add this file and all remaining files
            $filesToAdd += $file
            # Add all remaining files
            for ($i = $lines.IndexOf($line) + 1; $i -lt $lines.Count; $i++) {
                if ([string]::IsNullOrWhiteSpace($lines[$i])) { continue }
                $remainingFile = $lines[$i].Substring(3).Trim()
                $filesToAdd += $remainingFile
            }
            Write-Color "Adding all remaining files..." "Yellow"
            break
        }
        elseif ($addFile -eq 'Y' -or $addFile -eq 'y') {
            $filesToAdd += $file
            Write-Color "Added to staging" "Green"
        }
        else {
            Write-Color "Skipped" "Gray"
        }
    }
    
    if ($filesToAdd.Count -eq 0) {
        Write-Color "`nNo files selected to commit!" "Yellow"
        return
    }
    
    # Add selected files
    Write-Color "`nAdding $($filesToAdd.Count) file(s)..." "Yellow"
    foreach ($file in $filesToAdd) {
        git add $file
        if ($LASTEXITCODE -ne 0) {
            Write-Color "Failed to add: $file" "Red"
        }
    }
    Write-Color "Selected files added" "Green"
    
    # Commit with the message entered earlier
    Write-Color "`nCommitting with message: $commitMsg" "Yellow"
    git commit -m $commitMsg
    if ($LASTEXITCODE -ne 0) {
        Write-Color "Commit failed!" "Red"
        return
    }
    Write-Color "Commit successful" "Green"
    
    $branch = git rev-parse --abbrev-ref HEAD
    
    # Handle Detached HEAD
    if ($branch -eq "HEAD") {
        Write-Color "`nWarning: You are in 'Detached HEAD' state!" "Red"
        $fix = Read-Host "Merge these changes to 'main' branch? (Y/N)"
        if ($fix -eq 'Y' -or $fix -eq 'y') {
            $tempBranch = "temp_fix_$(Get-Date -Format 'yyyyMMddHHmmss')"
            git branch $tempBranch
            git checkout main
            git merge $tempBranch
            git branch -d $tempBranch
            $branch = "main"
            Write-Color "Switched to main and merged changes." "Green"
        } else {
            Write-Color "Cannot push from Detached HEAD. Please checkout a branch first." "Red"
            return
        }
    }

    Write-Color "`nCurrent branch: $branch" "Yellow"
    
    $pushConfirm = Read-Host "`nPush to remote? (Y/N)"
    if ($pushConfirm -ne 'Y' -and $pushConfirm -ne 'y') {
        Write-Color "Push cancelled, but commit completed" "Yellow"
        return
    }
    
    Write-Color "`nPushing to remote..." "Yellow"
    git push origin $branch
    
    if ($LASTEXITCODE -ne 0) {
        Write-Color "`nPush failed! May need to pull first or force push" "Red"
        $forcePush = Read-Host "Force push (will overwrite remote)? (Y/N)"
        if ($forcePush -eq 'Y' -or $forcePush -eq 'y') {
            Write-Color "Force pushing..." "Yellow"
            git push -f origin $branch
            if ($LASTEXITCODE -eq 0) {
                Write-Color "Force push successful!" "Green"
            }
            else {
                Write-Color "Force push failed!" "Red"
            }
        }
    }
    else {
        Write-Color "Push successful!" "Green"
    }
}

function Do-Push {
    Write-Color "`n========== Push Only ==========" "Cyan"
    
    $branch = git rev-parse --abbrev-ref HEAD
    
    if ($branch -eq "HEAD") {
        Write-Color "`nWarning: You are in 'Detached HEAD' state!" "Red"
        Write-Color "This implies your changes are not on a named branch." "Yellow"
        $fix = Read-Host "Merge changes to 'main' and push? (Y/N)"
        if ($fix -eq 'Y' -or $fix -eq 'y') {
            # Check for uncommitted changes
            $status = git status --porcelain
            $stashed = $false
            if ($status) {
                Write-Color "Stashing uncommitted changes..." "Yellow"
                git stash
                $stashed = $true
            }

            $tempBranch = "temp_fix_$(Get-Date -Format 'yyyyMMddHHmmss')"
            git branch $tempBranch
            git checkout main
            git merge $tempBranch
            git branch -d $tempBranch
            $branch = "main"
            
            if ($stashed) {
                git stash pop
            }
            Write-Color "Switched to main and merged changes." "Green"
        } else {
            Write-Color "Operation cancelled." "Yellow"
            return
        }
    }
    
    Write-Color "`nPushing branch '$branch' to remote..." "Yellow"
    git push origin $branch
    
    if ($LASTEXITCODE -ne 0) {
        Write-Color "`nPush failed!" "Red"
        $forcePush = Read-Host "Force push? (Y/N)"
        if ($forcePush -eq 'Y' -or $forcePush -eq 'y') {
            git push -f origin $branch
            if ($LASTEXITCODE -eq 0) {
                Write-Color "Force push successful!" "Green"
            }
        }
    } else {
        Write-Color "Push successful!" "Green"
    }
}

function Do-PullVersion {
    Write-Color "`n========== Pull Specific Version ==========" "Cyan"
    
    $status = git status --porcelain
    if ($status) {
        Write-Color "`nWarning: Uncommitted changes detected!" "Red"
        Write-Host $status
        $stash = Read-Host "`nStash these changes? (Y/N)"
        if ($stash -eq 'Y' -or $stash -eq 'y') {
            Write-Color "Stashing changes..." "Yellow"
            git stash push -m "Auto-stash $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
            Write-Color "Changes stashed" "Green"
        }
        else {
            Write-Color "Will force overwrite local changes" "Yellow"
        }
    }
    
    Write-Color "`nFetching remote info..." "Yellow"
    git fetch --all
    if ($LASTEXITCODE -ne 0) {
        Write-Color "Failed to fetch remote info!" "Red"
        return
    }
    
    Write-Host "`nSelect pull method:"
    Write-Host "1. Pull latest version (current branch)"
    Write-Host "2. Pull specific branch"
    Write-Host "3. Pull specific commit"
    Write-Host "4. Pull specific tag"
    Write-Host ""
    
    $choice = Read-Host "Choose (1-4)"
    
    switch ($choice) {
        "1" {
            $branch = git rev-parse --abbrev-ref HEAD
            Write-Color "`nPulling latest version (branch: $branch)..." "Yellow"
            git reset --hard origin/$branch
            git pull origin $branch
            Write-Color "Pull complete!" "Green"
        }
        "2" {
            Write-Host "`nAvailable remote branches:"
            git branch -r
            $branch = Read-Host "`nEnter branch name (e.g., main, master, develop)"
            
            Write-Color "`nSwitching and pulling branch: $branch..." "Yellow"
            $null = git checkout $branch 2>&1
            if ($LASTEXITCODE -ne 0) {
                git checkout -b $branch origin/$branch
            }
            git reset --hard origin/$branch
            git pull origin $branch
            Write-Color "Pull complete!" "Green"
        }
        "3" {
            Write-Host "`nRecent commits:"
            git log --oneline -20
            $commit = Read-Host "`nEnter commit hash (first 7 chars)"
            
            Write-Color "`nSwitching to commit: $commit..." "Yellow"
            git reset --hard $commit
            Write-Color "Switch complete!" "Green"
            Write-Color "Note: Currently in detached HEAD state" "Yellow"
        }
        "4" {
            Write-Host "`nAvailable tags:"
            git tag -l
            $tag = Read-Host "`nEnter tag name"
            
            Write-Color "`nSwitching to tag: $tag..." "Yellow"
            git checkout tags/$tag
            Write-Color "Switch complete!" "Green"
            Write-Color "Note: Currently in detached HEAD state" "Yellow"
        }
        default {
            Write-Color "Invalid choice!" "Red"
            return
        }
    }
    
    $clean = Read-Host "`nClean untracked files? (Y/N)"
    if ($clean -eq 'Y' -or $clean -eq 'y') {
        Write-Color "Cleaning..." "Yellow"
        git clean -fd
        Write-Color "Clean complete!" "Green"
    }
}

function Show-Status {
    Write-Color "`n========== Git Status ==========" "Cyan"
    
    $branch = git rev-parse --abbrev-ref HEAD
    Write-Color "`nCurrent branch: $branch" "Yellow"
    
    Write-Host "`nLatest commit:"
    git log -1 --oneline
    
    Write-Host "`nCurrent status:"
    git status
    
    Write-Host "`nRemote repository:"
    git remote -v
}

function Main {
    if (-not (Test-GitRepo)) {
        Write-Color "Error: Not a Git repository!" "Red"
        Read-Host "`nPress Enter to exit"
        exit
    }
    
    while ($true) {
        Show-Menu
        $choice = Read-Host "Choose function (1-5)"
        
        switch ($choice) {
            "1" { Do-CommitPush }
            "2" { Do-Push }
            "3" { Do-PullVersion }
            "4" { Show-Status }
            "5" { 
                Write-Color "`nGoodbye!" "Green"
                exit 
            }
            default { 
                Write-Color "`nInvalid choice, please try again!" "Red"
                Start-Sleep -Seconds 1
            }
        }
        
        if ($choice -ne "5") {
            Read-Host "`nPress Enter to continue"
        }
    }
}

Main
