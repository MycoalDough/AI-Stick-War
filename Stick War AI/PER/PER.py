from PER.sum_tree import SumTree as SumTree
import numpy as np


class PER_Memory(object):  # stored as ( state, action, reward, next_state ) in SumTree
    PER_e = 0.01  #hyper param used to make sure we don't select the same experience 2 times
    PER_a = 0.6  # trade off between exploration and explotation
    PER_b = 0.4  # importance-sampling, from initial value increasing to 1
   
    PER_b_increment_per_sampling = 4e-5
   
    absolute_error_upper = 1.  # clipped abs error


    def __init__(self, capacity):
        # Making the tree
        self.tree = SumTree(capacity)


    def store(self, experience):
        # Find the max priority
        max_priority = np.max(self.tree.tree[-self.tree.capacity:])


        # If the max priority = 0 we can't put priority = 0 since this experience will never have a chance to be selected
        # So we use a minimum priority
        if max_priority == 0:
            max_priority = self.absolute_error_upper


        self.tree.add(max_priority, experience)   # set the max priority for new priority


    def sample(self, n):
        minibatch = []


        b_idx = np.empty((n,), dtype=np.int32)


        self.PER_b = np.min([1., self.PER_b + self.PER_b_increment_per_sampling])


        # Calculate the priority segment
        # Here, as explained in the paper, we divide the Range[0, ptotal] into n ranges
        priority_segment = self.tree.total_priority / n       # priority segment


        for i in range(n):
            # A value is uniformly sample from each range
            a, b = priority_segment * i, priority_segment * (i + 1)
            value = np.random.uniform(a, b)


            # Experience that correspond to each value is retrieved
            index, priority, data = self.tree.get_leaf(value)


            b_idx[i]= index


            minibatch.append([data[0],data[1],data[2],data[3],data[4]])


        return b_idx, minibatch
   
    def batch_update(self, tree_idx, abs_errors):
        abs_errors += self.PER_e  # convert to abs and avoid 0
        clipped_errors = np.minimum(abs_errors, self.absolute_error_upper)
        ps = np.power(clipped_errors, self.PER_a)


        for ti, p in zip(tree_idx, ps):
            self.tree.update(ti, p)
