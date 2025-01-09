# Step 1: Find tracked files in obj/ and bin/ directories
git ls-files | Select-String -Pattern "obj/", "bin/" > files-to-remove.txt

# Step 2: Remove these files from Git index
Get-Content files-to-remove.txt | ForEach-Object {
    git rm --cached $_
}

# Step 3: Remove the file with the paths
rm files-to-remove.txt