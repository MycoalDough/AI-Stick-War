import torch
from PER.rainbow import RAINBOW_Q_Network

def test_noisy_layer():
    input_dims = (8,)
    n_actions = 4
    atom_size = 51
    support = torch.linspace(-1, 1, atom_size)

    net = RAINBOW_Q_Network(
        lr=0.001,
        input_dims=input_dims,
        n_actions=n_actions,
        checkpoint_dir='',
        name='test',
        atom_size=atom_size,
        support=support
    )

    input_tensor = torch.randn(1, input_dims[0])

    net.eval()
    output_before = net(input_tensor)
    print("Output before resetting noise:")
    print(output_before)

    net.reset_noise()
    output_after = net(input_tensor)
    print("Output after resetting noise:")
    print(output_after)

    is_different = not torch.allclose(output_before, output_after, atol=1e-6)
    print(f"Is the output different after resetting noise? {'Yes' if is_different else 'No'}")

test_noisy_layer()
