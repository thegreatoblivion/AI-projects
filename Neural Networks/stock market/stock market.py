import torch
from torch import nn
import numpy as np
from torchvision import datasets, transforms
import matplotlib.pyplot as plt
import torch
import csv
import numpy as np
import tensorflow as tf
import math

with open(r'C:\Users\user\Documents\road to qm\machine learning\pytorch\stock market\NVDA_10.csv', newline='') as csvfile:
    data = list(csv.reader(csvfile))
data = np.array(data)
#lookback_range = 1000
#data = data[:lookback_range,:]
num_days = data.shape[0]-1

#print(num_days)
print(data)
print(data.shape)
def normalize(n):
    n = 1/(1+np.exp(-n))
    return n
def stock_matrix_generator(data, days, batch_size):  #generates a matrix of stock prices with non-repeating batches
    data = np.delete(data,0,axis = 0) #deletes the top row
    upper_range = np.random.randint(num_days - days, num_days) #gives index between  num_days and num_Days - days as a starting point.
    #this selects the starting point for the batch to cycle over
    stock_matrix = []
    next_day_matrix = []
    lower_range = upper_range - days
    #closing value the next day - this will be our test data
    
    
   
    #slicing data into small chunks and removing unneccessary rows and columns
    for i in range(batch_size):
       
        
        next_day_row = data[lower_range-(i*days)-1]
        next_day_row = next_day_row[:2]
        print('next day row')
        print(next_day_row)
        next_day_matrix.append(next_day_row)
        print('next day matrix')
        print(next_day_matrix)
        stock_array = data[lower_range-(i*days):upper_range-(i*days),:]
        stock_array = stock_array[:,1] #only need closing price

        #now to get rid of the dollar sign and change all into floats
        for i in range(len(stock_array)):
            price = stock_array[i]
            price = price[1:]
            stock_array[i] = price
        stock_array = np.array([list(map(float,stock_array))])

        #now turn into a single lined vector
        stock_vector = stock_array.reshape(days,1)
        stock_matrix.append(stock_vector)
        
        
    return stock_matrix, next_day_matrix
#initializing
days = 20
batch_size = math.floor(num_days/days)-1
print(batch_size)

stock_matrix, next_day_matrix = stock_matrix_generator(data,days, batch_size)
stock_matrix = np.array(stock_matrix)
next_day_matrix = np.array(next_day_matrix)

print(next_day_matrix)
#stock_tensor = torch.from_numpy(stock_vector)
#actual = torch.from_numpy(np.array([float(next_day_row[1][1:])])).float()