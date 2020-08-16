# WebScraping - Getting Started

Clone the repository and then compile and run the WebScraping.Api project.

The application will open the swagger page automatically. Then just insert the url of a public github repository in the input parameter <b>urlGitHub</b>. Example:

> ![Swagger](https://i.ibb.co/r4Bm145/swagger.png)


# Output 
The request will be processed and may take a few minutes depending on the number of files in the repository. Subsequent requests for the same repository url are faster.
The return will look something like this:

> ![Swagger Response](https://i.ibb.co/hVyZZwH/response-swagger.png)

Note that the status code is 200 and the success property is true. 
This means that the data object returned contains a list of objects representing the total lines and the total bytes referring to files with that type of extension.
