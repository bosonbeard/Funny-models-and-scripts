Version 1.0 (consist data from 2016 to August 2019)

A set of data on citizens' appeals to the executive power of the city of Moscow (https://www.mos.ru/feedback/reviews/),
for details see msc_app_to_mayor.ipynb or https://habrahabr.ru/post/343216/

____________________________________________________________________________________________________________
In Data folder:
msc_appel_data2.csv is collected manual dataset  from  https://www.mos.ru/feedback/reviews/, where:  

num - Record index  
year is the year of recording  
month - recording month  
total_appeals - total number of hits per month  

appeals_to_mayor - total number of appeals to the Mayor  
res_positive - the number of appeals with positive decisions  
res_explained - the number of appeals that were explained  
res_negative - number of appeals with negative decision  
El_form_to_mayor - the number of appeals to the Mayor in electronic form  
Pap_form_to_mayor - - number of appeals to the Mayor on paper  
 to_10K_total_VAO ... to_10K_total_YUZAO - the number of appeals per 10000 population in various districts of Moscow  
to_10K_mayor_VAO ... to_10K_mayor_YUZAO- the number of appeals to the Mayor and the Government of Moscow for 10,000 people in various districts of the city  

test_data and train_data files - edited versions of msc_appel_data2.csv  for  used in app_to_mayor_mlnet.csproj

__________________________________________________________________________________  

The  solution   is an additional material to the article 
demonstrating examples of data analysis and linear regression on C# with Ml.NET Framework. 
More detailed on the Habrahabr - https://habr.com/ru/post/473342/
Materials may contain errors, not recommended for serious research.
