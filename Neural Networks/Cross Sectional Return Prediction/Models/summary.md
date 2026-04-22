
    #Model Name: deep neural network 

    Created: 2026-04-20_23-03

    ## Config
    - Epochs: 2
    - Batch size: 1024
    - Learning rate: 0.001
    - Hidden Units: [64, 32, 16]
    - Target Parameters: ['mom_12_1 lag', 'norm_SMA_12 lag', 'turnover lag', 'volatility_12 lag', 'E/P lag']

    ## Notes
    deep neural network with 2 epochs, seemingly top_pct and bot_pct now needs to be 0.95,0.05 to get good sharpe ratio
    interestingly the momentum coefficient is a lot less significant, suggesting it is less correlated with the momentum!
    #Performance: 
    |                       |   Train Dataset |   Test Dataset |   Whole Dataset |
|:----------------------|----------------:|---------------:|----------------:|
| Sharpe Ratio          |        1.0242   |       0.820642 |        0.999132 |
| Average Yearly Return |        0.284237 |       0.265375 |        0.282462 |
| Top %                 |        0.95     |       0.95     |        0.95     |
| Bottom %              |        0.05     |       0.05     |        0.05     |

## OLS Regression

```
                            OLS Regression Results                            
==============================================================================
Dep. Variable:                      y   R-squared:                       0.054
Model:                            OLS   Adj. R-squared:                  0.052
Method:                 Least Squares   F-statistic:                     25.61
Date:                Mon, 20 Apr 2026   Prob (F-statistic):           1.54e-11
Time:                        23:02:57   Log-Likelihood:                 1131.2
No. Observations:                 893   AIC:                            -2256.
Df Residuals:                     890   BIC:                            -2242.
Df Model:                           2                                         
Covariance Type:            nonrobust                                         
==============================================================================
                 coef    std err          t      P>|t|      [0.025      0.975]
------------------------------------------------------------------------------
const          0.0201      0.002      8.553      0.000       0.016       0.025
Mkt-RF        -0.2520      0.053     -4.715      0.000      -0.357      -0.147
Mom            0.2688      0.059      4.573      0.000       0.153       0.384
==============================================================================
Omnibus:                      107.292   Durbin-Watson:                   1.798
Prob(Omnibus):                  0.000   Jarque-Bera (JB):              728.362
Skew:                          -0.287   Prob(JB):                    6.89e-159
Kurtosis:                       7.387   Cond. No.                         26.7
==============================================================================

Notes:
[1] Standard Errors assume that the covariance matrix of the errors is correctly specified.
```
