# CitationVK
The following document contains sections of the report that accompanied this project, demonstrating site functionality with screenshots.

## Background
During my co-op term I worked on an application that helps to create optimal search strings in order to gather sets of relevant citations from medical literature databases such as PubMed (https://www.ncbi.nlm.nih.gov/pubmed/). Based off earlier research, this application generates search terms using a brute-force approach by testing millions of combinations of phrases against a known set of positive and negative results. While working on this, I began to wonder if a machine-learning approach had ever been applied to the same problem. I quickly found that it had in fact been attempted, but is "not used much in practice, in part because the best way to make use of the technology in a typical workflow is unclear" (https://www.ncbi.nlm.nih.gov/pmc/articles/PMC6030513/). For my capstone, I hope to develop a web application that creates this very workflow. The application will use one of the many available machine learning libraries such as ML.NET or NLTK for generating the algorithms from the given data set. The relative success or failure of the algorithms is unimportant, as the focus is to create a user-friendly interface that could provide an adequate workflow that could be used by anybody, regardless of their experience in machine learning. The application would require the following features: a log-in system, the ability to upload training sets and generate algorithms as well as evaluate new sets and display the results.

## Description
The application will facilitate the creation and usage of machine-learning models to identify research papers found in the PubMed database (https://en.wikipedia.org/wiki/PubMed) that match a desired category. The type of machine-learning models the application will generate are called Binary Classification models, and they will be able to determine if new articles either fit into the desired category or not (https://en.wikipedia.org/wiki/Binary_classification). Examples of categories could be randomized controlled trials, cancer treatment, or gene therapy. The desired category is entirely up to the user: models will be trained on the data that they provide. Once a model has been created, it can then be presented with a list of novel papers and attempt to classify them accordingly.

This project was developed using the Microsoft .NET Razor Pages framework, with some MVC elements. It makes use of the Entrez E-utilities API (https://www.ncbi.nlm.nih.gov/books/NBK25500/) to retrieve articles from the PubMed database (https://www.ncbi.nlm.nih.gov/pubmed/). Machine learning tasks were carried out using the Microsoft ML.NET plug-in (https://dotnet.microsoft.com/learn/ml-dotnet). Table searching and pagination functionalities were achieved using the DataTables JavaScript plug-in (https://datatables.net/). The front-end uses the Bootstrap 4.0 framework (https://getbootstrap.com/). The jQuery JavaScript library (https://jquery.com/) was also used to support some functions.

## Database Schema
![Database Schema](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture20.png)

## Usage

![Figure 1](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture1.png)

Figure 1: All users are able to view the About page which contains the history and goals of the application.

![Figure 2](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture2.png)

Figure 2: Users who are not yet registered and signed in will are able to register using an email address, if public account registration is enabled by an administrator. New users must also pick a security question and answer for password recovery.

![Figure 3](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture3.png)

Figure 3: Users can sign in using their email address and password on this page, as well as recover their password.

![Figure 4](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture4.png)

Figure 4: Users who have forgotten their password can recover their account by answering their security question and setting a new password.

![Figure 5](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture5.png)

Figure 5: When viewing the About page on an admin account, the “Edit” button will be visible.

![Figure 6](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture6.png)

Figure 6: From the edit page, administrators can edit the contents of the about page and enable or disable public account creation.

![Figure 7](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture7.png)

Figure 7: Administrators can view a list of all accounts with tools to edit and delete user accounts, including elevation to administrator status.

![Figure 8](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture8.png)

Figure 8: User accounts can also be manually created by administrators by clicking the Create Account button on the accounts page.

![Figure 9](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture9.png)

Figure 9: Administrators can edit accounts by clicking the edit tool, including the ability to give them administrator privileges. Registered users are also able to edit their own account.

![Figure 10](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture10.png)

Figure 10: From this table, users will be able to view, create, modify, download and share article data sets.

![Figure 11](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture11.png)

Figure 11: New sets are created by entering a name, and adding article IDs and their classifications in text form or by uploading a .csv file.

![Figure 12](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture12.png)

Figure 12: Users will be able to expand data sets through merging any two data sets by clicking the merge link on the datasets page.

![Figure 13](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture13.png)

Figure 13: After clicking on the edit tool for a dataset, users are taken to the data set details page where they can view information for each article contained within. Data sets can also be modified from this page: articles can be deleted or have their classifications reversed, and the name of the set can be changed. Each article title is a hyperlink to the articles page on PubMed.

![Figure 14](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture14.png)

Figure 14: Data sets can be shared with other users, shared data sets will be viewable and editable by other users.

![Figure 15](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture15.png)

Figure 15: Users will be able create, view, modify, share and train new classifiers on the Classifiers page. From here users are also able to view and compare important classifier statistics such as accuracy, precision and recall.

![Figure 16](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture16.png)

Figure 16: New machine-learning classifiers must be given a name, and are trained using an existing data set. The type of machine-learning models the application will generate are called Binary Classification models, and they will be able to determine if new articles either fit into the desired category or not (https://en.wikipedia.org/wiki/Binary_classification). Examples of categories could be randomized controlled trials, cancer treatment, or gene therapy. The desired category is entirely up to the user: models will be trained on the data that they provide.

![Figure 17](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture17.png)

Figure 17: Classifiers are generated by the machine learning library ML.NET, these models are also known as binary classification algorithms. Once trained, classifiers can then be used to classify a set of new articles. Results are automatically compiled into a new dataset with the given name.

![Figure 18](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture18.png)

Figure 18: Trained classifiers can easily be shared with other users by clicking the share link. An example of a classifier would be a binary classification algorithm that determines whether or not a specific research paper is related to a topic. For example, an article that contains the words “pancreatic cancer” would likely be classified as a positive result for an algorithm trained to find articles about cancer.

![Figure 19](https://raw.githubusercontent.com/NigelReimer/images/main/CitationVK/Picture19.png)

Figure 19: Classifying new article sets is carried out on the Classify Articles page. Users enter a list of PubMed IDs, similar to creating datasets but without a manual classification. Instead, the selected classifier classifies the articles automatically and creates a new dataset with the given name. These datasets can then be used for research, or manually verified and merged with existing datasets in order to improve the efficacy of new classifiers trained in the future.
