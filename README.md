Description:

Enhance your ASP.NET Web API projects with this robust and easy-to-use library that provides a generic repository and controller for seamless CRUD operations.

This package is designed to simplify the development process by offering out-of-the-box implementation of basic Create, Read, Update, and Delete (CRUD) functionalities,
allowing developers to focus more on business logic and less on boilerplate code.

Features:

Generic Repository: A versatile and reusable repository pattern
  that supports common CRUD operations for any entity type.

Generic Controller: A ready-to-use controller with endpoints for handling CRUD operations,
  reducing the need to write repetitive code for each entity.

Easy Integration: Simple and straightforward integration into any ASP.NET Web API project,
  with minimal configuration required.
  
Extensible: Easily extendable to cater to specific business requirements and additional functionalities.

Entity-agnostic: Works with any entity type, making it a flexible choice for diverse applications.


how to use:

1- all you need to do is to make your repository inherits from the BaseRepository Class 
![image](https://github.com/vodkagamed/Generic_Server/assets/89106400/f6754a61-ccb7-48ea-b39d-c4c46ff4e13a)


2- Next you will have to make your controller inherits from BaseController and pass your injected Repository
![image](https://github.com/vodkagamed/Generic_Server/assets/89106400/7af16527-1082-4f94-8963-544f0134bc85)

3- and you just got your CRUD operations ready to use!! 
![image](https://github.com/vodkagamed/Generic_Server/assets/89106400/62d032f6-8e59-48b1-99bc-8b8dd4e6ae46)
