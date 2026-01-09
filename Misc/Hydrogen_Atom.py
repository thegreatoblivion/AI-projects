import scipy as scipy
import matplotlib.pyplot as plt
import numpy as np
from scipy.special import factorial
import plotly.graph_objects as go
import dash
from dash import Dash, dcc, html, Input, Output

def Y_norm(l,m,theta,phi):
    #first input is l value, second input is m value, 3rd is theta, 4th is phi
    return scipy.special.sph_harm_y(l,m,theta,phi)

def PSI(n,l,m,r,theta,phi):
    L = scipy.special.genlaguerre(n-l-1,2*l+1)

    normFactor = np.sqrt((8*factorial(n-l-1))/(n**3*2*n*factorial(n+l)))
    p = 2*r/n
    return normFactor*np.exp(-p/2)*p**l*L(p)*Y_norm(l,m,theta,phi)

def IsNormalizedPolar(function):
    r_limits = [0, np.inf]
    theta_limits = [0, np.pi]
    phi_limits = [0, 2*np.pi]
    def Integrand(r,theta,phi): 
        return np.abs(function(n,l,m,r,theta,phi))**2*r**2*np.sin(theta)
    I = scipy.integrate.nquad(Integrand, [r_limits,theta_limits,phi_limits])
    #I = I[0] +- I[1]  (I[1] is the error interval)
    return (I[0]+I[1]>=1) and (I[0] - I[1] <= 1)



#R, Theta, Phi = np.mgrid[0:1:4j, 0:np.pi:4j, 0:2*np.pi:4j]
#NOW R[i,j,k] refers to the r value at the [i,j,k] point in the 3d grid. The point represents
# the ith value in r array, jth value in theta array, kth value in phi array.
#i j and k goes from 0 to 40
#Because now we have 40x40x40 different points instead of 40 initially
#Tensor product?

#Now if we pass any function f(R,Theta, Phi) = R**2 + sin(Phi) then the value at [i,j,k] is
# f(R, Theta, Phi)[i,j,k] = (R[i,j,k])**2 + sin(Phi[i,j,k])  --> everything should be elementwise
# so this returns values over all the possible 40x40x40 points.
#F = np.abs(PSI(R,Theta,Phi))**2
#need to convert to X Y Z for plotly to work
#X = R * np.sin(Theta) * np.cos(Phi)
#Y = R * np.sin(Theta) * np.sin(Phi)
#Z = R * np.cos(Theta) 
#oh, but actually none of the above works because plotly doesnt allow non uniform grids (??) and I should
#use a function of X Y Z no matter what?

app = Dash(__name__)


app.layout = html.Div([
    html.H3("Hydrogen Atom Wavefunction",style={'color': 'blue !important'}),
    
    html.Div([
        html.Label("Energy number n:"),
        dcc.Slider(id='n-slider', min=1, max=10, step=1,value = 1),
    ], style={'padding': '20px'}),

    html.Div([
        html.Label("l:"),
        dcc.Slider(id='l-slider', min=0.0, max=0, step=1, value = 0),
    ], style={'padding': '20px'}),
    
    html.Div([
        html.Label("m:"),
        dcc.Slider(id='m-slider', min=-0, max=0, step=1, value = 0),
    ], style={'padding': '20px'}),

    dcc.Graph(id='wavefunction')
], style={'color': 'blue'})
#call back to update bounds of l when n is updated
@app.callback(
    [Output('l-slider', 'max'),
     Output('l-slider', 'value')],
    [Input('n-slider', 'value'),
     Input('l-slider', 'value')]
)
def update_slider_range(n,l):

    l_max = n - 1
    l =  l_max if l > l_max else l
    return (l_max, l)

#callback to update bounds of m when l is updated
@app.callback(
    [
     Output('m-slider', 'max'),
     Output('m-slider', 'min'),
     Output('m-slider', 'value')],
    [Input('l-slider', 'value'),
     Input('m-slider', 'value')]
)
def update_slider_range(l,m):
   
    m_max = l
    m_min = -l
    m = l*np.sign(m) if np.abs(m) > l else m
    return (m_max, m_min, m)

#call back to update the whole graph
@app.callback(
    Output('wavefunction', 'figure'),
    [Input('n-slider', 'value'),
     Input('l-slider', 'value'),
     Input('m-slider', 'value')
     ]
)
def update_graph(n,l,m):

    range = (2/3)*(3*n**2 - l*(l-1) )
    #expectation value of r is equal to 1/2(3*n**2 - l*(l-1) )
    num_points = 50
    #NUMBER OF POINTS IN TOTAL IS NUM_POINTS^3 BE CAREFUL
    X,Y,Z = np.mgrid[-range:range:num_points*1j, -range:range:num_points*1j, -range:range:num_points*1j]
    R = np.sqrt(X**2 + Y**2 + Z**2)
    Theta = np.where(R !=0 , np.arccos(Z/R), 0)
    #Theta = np.arccos(Z/R)
    Phi = np.where((Y !=0) & (X != 0), np.arctan(Y/X), np.pi/2*np.sign(Y))
    #Phi = np.arctan(Y/X)
    # if x and y 0 phi = 0, if only x=0 then phi = +-pi/2 depending on its sign

    F = np.abs(PSI(n,l,m,R,Theta,Phi))**2



    fig = go.Figure(
        data = go.Volume(
            x=X.flatten(),
            y=Y.flatten(),
            z=Z.flatten(),
            value=F.flatten(),
            colorscale="Viridis",
            opacity=0.2,
            surface_count=20,

        ))

    #flatten because plotly only wants 1d lists
    return fig

if __name__ == '__main__':
    app.run()