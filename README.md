[![.NET Test](https://github.com/ITU-BDSA2024-GROUP12/Chirp/actions/workflows/test.yaml/badge.svg)](https://github.com/ITU-BDSA2024-GROUP12/Chirp/actions/workflows/test.yaml)[![.NET Build](https://github.com/ITU-BDSA2024-GROUP12/Chirp/actions/workflows/build.yaml/badge.svg)](https://github.com/ITU-BDSA2024-GROUP12/Chirp/actions/workflows/build.yaml)

# Chirp
Chirp is a website where authors can post messages in form of cheeps, and read other authors' messages.

# Running application from release files

If you want to run the _Chirp!_ application from the executable published in the release.
You will need to set the secrets for Github OAuth as environment variables, on your system, before running the executable. 

# Dependency

- Microsoft EF Core v7.0.0
- Microsoft EF Core Design v7.0.0
- Microsoft EF Core SQLite v7.0.0

# Creating new release

Tag the commit by using:
`git tag -a v*.*.* -m "my version...."`

then push the commit and tag
`git push origin v*.*.*`

# Co-authors

Co-authored-by: Peter <pblo@itu.dk>
Co-authored-by: Rasmus <rsni@itu.dk>
Co-authored-by: Mathias <bamj@itu.dk>
Co-authored-by: Christian <cmol@itu.dk>
Co-authored-by: Johannes <johje@itu.dk>

# Report PDF

To find the report artifact: click the Actions tab -> Click the "Publish Report" workflow on the lefthand side -> Click on the newest workflow.
