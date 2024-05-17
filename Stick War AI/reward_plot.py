import matplotlib.pyplot as plt

class RewardPlotter:
    def __init__(self, max_points=50, title="Real-time Reward Plot", figsize=(5, 3)):
        self.fig, self.ax = plt.subplots(figsize=figsize)
        self.fig.canvas.manager.set_window_title(title)  # Ensure unique titles
        self.line, = self.ax.plot([], [], 'r-')
        self.x_data, self.y_data = [], []
        self.max_points = max_points
        plt.ion()
        self.ax.set_title(title)
        self.ax.set_xlabel("Time Steps")
        self.ax.set_ylabel("Reward")

    def update(self, reward):
        current_index = len(self.x_data)  # Current total number of updates
        self.x_data.append(current_index)
        self.y_data.append(reward)

        # Trim the data to maintain only the last `max_points` entries
        if len(self.x_data) > self.max_points:
            min_x = self.x_data[0] + 1  # Increment the minimum x to maintain alignment
            self.x_data = self.x_data[-self.max_points:]
            self.y_data = self.y_data[-self.max_points:]
            # Correct the x_data to reflect actual indices
            self.x_data = list(range(min_x, min_x + self.max_points))

        # Update the plot with new data
        self.line.set_xdata(self.x_data)
        self.line.set_ydata(self.y_data)

        # Set the x-axis limits to include the last `max_points` updates
        self.ax.set_xlim(left=self.x_data[0], right=self.x_data[-1] + 1)

        self.ax.relim()
        self.ax.autoscale_view(True, True, True)
        self.fig.canvas.draw_idle()
        plt.pause(0.001)


    def close(self):
        plt.ioff()
        self.fig.show()
